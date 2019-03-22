// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace MaikeBing.EntityFrameworkCore.LiteDB.Storage.Internal
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
            => _factories.GetOrAdd(entityType.FindPrimaryKey(), Create)();

        private Func<ILiteDBTable> Create([NotNull] IKey key)
            => (Func<ILiteDBTable>)typeof(LiteDBTableFactory).GetTypeInfo()
                .GetDeclaredMethod(nameof(CreateFactory))
                .MakeGenericMethod(GetKeyType(key))
                .Invoke(null, new object[] { key, _sensitiveLoggingEnabled });

        [UsedImplicitly]
        private static Func<ILiteDBTable> CreateFactory<TKey>(IKey key, bool sensitiveLoggingEnabled)
            => () => new LiteDBTable<TKey>(key.GetPrincipalKeyValueFactory<TKey>(), sensitiveLoggingEnabled);
    }
}
