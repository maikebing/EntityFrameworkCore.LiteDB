// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Reflection;
using JetBrains.Annotations;
using LiteDB;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace MaikeBing.EntityFrameworkCore.NoSqLiteDB.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class LiteDBTableFactory : IdentityMapFactoryFactoryBase, ILiteDBTableFactory
    {
        private readonly bool _sensitiveLoggingEnabled;

        private readonly ConcurrentDictionary<IKey, Func<ILiteDBTable>> _factories
            = new ConcurrentDictionary<IKey, Func<ILiteDBTable>>();

        public LiteDatabase LiteDatabase { get; set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public LiteDBTableFactory([NotNull] ILoggingOptions loggingOptions)
        {
            Check.NotNull(loggingOptions, nameof(loggingOptions));

            _sensitiveLoggingEnabled = loggingOptions.IsSensitiveDataLoggingEnabled;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual ILiteDBTable Create(IEntityType entityType)
        {
            return _factories.GetOrAdd(entityType.FindPrimaryKey(),  Create(entityType.FindPrimaryKey(), entityType))();
        }

        private Func<ILiteDBTable> Create([NotNull] IKey key, IEntityType entityType)
            => (Func<ILiteDBTable>)typeof(LiteDBTableFactory).GetTypeInfo()
                .GetDeclaredMethod(nameof(CreateFactory))
                .MakeGenericMethod(GetKeyType(key))
                .Invoke(null, new object[] { key, _sensitiveLoggingEnabled  ,LiteDatabase, entityType });

        [UsedImplicitly]
        private static Func<ILiteDBTable> CreateFactory<TKey>(IKey key, bool sensitiveLoggingEnabled,LiteDB.LiteDatabase _liteDatabase,IEntityType _entityType)
            => () => new LiteDBTable<TKey>(key.GetPrincipalKeyValueFactory<TKey>(), sensitiveLoggingEnabled, _liteDatabase, _entityType);
    }
}
