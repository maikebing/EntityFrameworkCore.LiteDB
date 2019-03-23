// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using MaikeBing.EntityFrameworkCore.NoSqLiteDB.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore;
using LiteDB;

namespace MaikeBing.EntityFrameworkCore.NoSqLiteDB.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class LiteDBTable<TKey> : ILiteDBTable
    {
        private readonly IPrincipalKeyValueFactory<TKey> _keyValueFactory;
        private readonly bool _sensitiveLoggingEnabled;
        private readonly LiteDB.LiteCollection<BsonDocument> _docrows;
        private readonly IEntityType _entityType;
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public LiteDBTable([NotNull] IPrincipalKeyValueFactory<TKey> keyValueFactory, bool sensitiveLoggingEnabled, LiteDatabase _liteDatabase,IEntityType  entityType)
        {
            _keyValueFactory = keyValueFactory;
            _sensitiveLoggingEnabled = sensitiveLoggingEnabled;
            _docrows = _liteDatabase.GetCollection<BsonDocument>(entityType.TableName());
            entityType.GetKeys()?.ToList().ForEach(key =>
            {
                key.Properties.ToList().ForEach(ip =>
                {
                    _docrows.EnsureIndex(ip.Name, ip.IsForeignKey());
                });
            });
            _entityType = entityType;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IReadOnlyList<object[]> SnapshotRows()
        {
            List<object[]> vs = new List<object[]>();
            _docrows.FindAll().ToList().ForEach(doc =>
                {
                    var value = BsonMapper.Global.ToObject(_entityType.ClrType, doc);
                    vs.Add(_entityType.GetProperties().Select(p => p.PropertyInfo.GetValue(value)).ToArray());
                });
            return vs;
        }

        private static List<ValueComparer> GetStructuralComparers(IEnumerable<IProperty> properties)
            => properties.Select(GetStructuralComparer).ToList();

        private static ValueComparer GetStructuralComparer(IProperty p)
            => p.GetStructuralValueComparer() ?? p.FindMapping()?.StructuralComparer;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void Create(IUpdateEntry entry)
        {
            var key = CreateKey(entry);
            _docrows.Insert(BsonMapper.Global.ToDocument(entry.ToEntityEntry().Entity));
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void Delete(IUpdateEntry entry)
        {
            var key = CreateKey(entry);
           var value=  _docrows.FindById(new BsonValue( key));
            if ( value!=null)
            {
                _docrows.Delete(new BsonValue(key));
            }
            else
            {
                throw new DbUpdateConcurrencyException(LiteDBStrings.UpdateConcurrencyException, new[] { entry });
            }
        }

        private static bool IsConcurrencyConflict(
            IUpdateEntry entry,
            IProperty property,
            object rowValue,
            Dictionary<IProperty, object> concurrencyConflicts)
        {
            if (property.IsConcurrencyToken
                && !StructuralComparisons.StructuralEqualityComparer.Equals(
                    rowValue,
                    entry.GetOriginalValue(property)))
            {
                concurrencyConflicts.Add(property, rowValue);

                return true;
            }

            return false;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void Update(IUpdateEntry entry)
        {
            var key = CreateKey(entry);
            var value = _docrows.FindById(new BsonValue(key));
            if (value!=null)
            {
                var properties = entry.EntityType.GetProperties().ToList();
                var comparers = GetStructuralComparers(properties);
                var valueBuffer = new object[properties.Count];
                var concurrencyConflicts = new Dictionary<IProperty, object>();
                for (int i = 0; i < properties.Count; i++)
                {
                    if (entry.IsModified(properties[i]))
                    {
                        value.Set(properties[i].Name, new BsonValue(SnapshotValue(properties[i], comparers[i], entry)));
                    }
                }
                _docrows.Update( new BsonValue(key), BsonMapper.Global.ToDocument(value));
            }
            else
            {
                throw new DbUpdateConcurrencyException(LiteDBStrings.UpdateConcurrencyException, new[] { entry });
            }
        }

        private TKey CreateKey(IUpdateEntry entry)
            => _keyValueFactory.CreateFromCurrentValues((InternalEntityEntry)entry);

        private static object SnapshotValue(IProperty property, ValueComparer comparer, IUpdateEntry entry)
            => SnapshotValue(comparer, entry.GetCurrentValue(property));

        private static object SnapshotValue(ValueComparer comparer, object value)
            => comparer == null ? value : comparer.Snapshot(value);

        /// <summary>
        ///     Throws an exception indicating that concurrency conflicts were detected.
        /// </summary>
        /// <param name="entry"> The update entry which resulted in the conflict(s). </param>
        /// <param name="concurrencyConflicts"> The conflicting properties with their associated database values. </param>
        protected virtual void ThrowUpdateConcurrencyException([NotNull] IUpdateEntry entry, [NotNull] Dictionary<IProperty, object> concurrencyConflicts)
        {
            Check.NotNull(entry, nameof(entry));
            Check.NotNull(concurrencyConflicts, nameof(concurrencyConflicts));

            if (_sensitiveLoggingEnabled)
            {
                throw new DbUpdateConcurrencyException(
                    LiteDBStrings.UpdateConcurrencyTokenExceptionSensitive(
                        entry.EntityType.DisplayName(),
                        entry.BuildCurrentValuesString(entry.EntityType.FindPrimaryKey().Properties),
                        entry.BuildOriginalValuesString(concurrencyConflicts.Keys),
                        "{" + string.Join(", ", concurrencyConflicts.Select(c => c.Key.Name + ": " + Convert.ToString(c.Value, CultureInfo.InvariantCulture))) + "}"),
                    new[] { entry });
            }

            throw new DbUpdateConcurrencyException(
                LiteDBStrings.UpdateConcurrencyTokenException(
                    entry.EntityType.DisplayName(),
                    Property.Format(concurrencyConflicts.Keys)),
                new[] { entry });
        }
    }
}
