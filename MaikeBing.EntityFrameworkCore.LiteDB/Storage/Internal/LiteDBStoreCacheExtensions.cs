// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MaikeBing.EntityFrameworkCore.LiteDB.Infrastructure.Internal;

namespace MaikeBing.EntityFrameworkCore.LiteDB.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public static class LiteDBStoreCacheExtensions
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static ILiteDBStore GetStore([NotNull] this ILiteDBStoreCache storeCache, [NotNull] IDbContextOptions options)
            => storeCache.GetStore(options.Extensions.OfType<LiteDBOptionsExtension>().First().StoreName);
    }
}
