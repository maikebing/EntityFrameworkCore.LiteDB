// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using MaikeBing.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace MaikeBing.EntityFrameworkCore.LiteDB.Infrastructure.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class LiteDBSingletonOptions : ILiteDBSingletonOptions
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void Initialize(IDbContextOptions options)
        {
            var inMemoryOptions = options.FindExtension<LiteDBOptionsExtension>();

            if (inMemoryOptions != null)
            {
                DatabaseRoot = inMemoryOptions.DatabaseRoot;
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void Validate(IDbContextOptions options)
        {
            var inMemoryOptions = options.FindExtension<LiteDBOptionsExtension>();

            if (inMemoryOptions != null
                && DatabaseRoot != inMemoryOptions.DatabaseRoot)
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(LiteDBDbContextOptionsExtensions.UseLiteDB),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual LiteDBDatabaseRoot DatabaseRoot { get; private set; }
    }
}
