// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Threading;
using JetBrains.Annotations;
using MaikeBing.EntityFrameworkCore.LiteDB.Infrastructure.Internal;

namespace MaikeBing.EntityFrameworkCore.LiteDB.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class LiteDBStoreCache : ILiteDBStoreCache
    {
        private readonly ILiteDBTableFactory _tableFactory;
        private readonly bool _useNameMatching;
        private readonly ConcurrentDictionary<string, ILiteDBStore> _namedStores;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Obsolete("Use the constructor that also accepts options.")]
        public LiteDBStoreCache([NotNull] ILiteDBTableFactory tableFactory)
            : this(tableFactory, null)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public LiteDBStoreCache(
            [NotNull] ILiteDBTableFactory tableFactory,
            [CanBeNull] ILiteDBSingletonOptions options)
        {
            _tableFactory = tableFactory;

            if (options?.DatabaseRoot != null)
            {
                _useNameMatching = true;

                LazyInitializer.EnsureInitialized(
                    ref options.DatabaseRoot.Instance,
                    () => new ConcurrentDictionary<string, ILiteDBStore>());

                _namedStores = (ConcurrentDictionary<string, ILiteDBStore>)options.DatabaseRoot.Instance;
            }
            else
            {
                _namedStores = new ConcurrentDictionary<string, ILiteDBStore>();
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual ILiteDBStore GetStore(string name)
            => _namedStores.GetOrAdd(name, _ => new LiteDBStore(_tableFactory, _useNameMatching));
    }
}
