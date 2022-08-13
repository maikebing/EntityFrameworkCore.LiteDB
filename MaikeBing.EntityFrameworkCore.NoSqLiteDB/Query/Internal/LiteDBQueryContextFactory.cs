// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MaikeBing.EntityFrameworkCore.NoSqLiteDB.Storage.Internal;
using Microsoft.EntityFrameworkCore.Query;

namespace MaikeBing.EntityFrameworkCore.NoSqLiteDB.Query.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class LiteDBQueryContextFactory : IQueryContextFactory 
    {
        private readonly ILiteDBStore _store;
        private readonly QueryContextDependencies _dependencies;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public LiteDBQueryContextFactory(
            [NotNull] QueryContextDependencies dependencies,
            [NotNull] ILiteDBStoreCache storeCache,
            [NotNull] IDbContextOptions contextOptions)
        {
            _store = storeCache.GetStore(contextOptions);
            _dependencies = dependencies;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public   QueryContext Create()
            => new LiteDBQueryContext(_dependencies);
    }
}
