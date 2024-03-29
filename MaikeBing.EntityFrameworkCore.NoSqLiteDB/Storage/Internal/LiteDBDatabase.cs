// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Utilities;

namespace MaikeBing.EntityFrameworkCore.NoSqLiteDB.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class LiteDBDatabase : Database, ILiteDBDatabase
    {
        private readonly ILiteDBStore _store;
        private readonly IDiagnosticsLogger<DbLoggerCategory.Update> _updateLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteDBDatabase"/> class.
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public LiteDBDatabase(
            [NotNull] DatabaseDependencies dependencies,
            [NotNull] ILiteDBStoreCache storeCache,
            [NotNull] IDbContextOptions options,
            [NotNull] IDiagnosticsLogger<DbLoggerCategory.Update> updateLogger)
            : base(dependencies)
        {
            Check.NotNull(storeCache, nameof(storeCache));
            Check.NotNull(options, nameof(options));
            Check.NotNull(updateLogger, nameof(updateLogger));

            _store = storeCache.GetStore(options);
            _updateLogger = updateLogger;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual ILiteDBStore Store => _store;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override int SaveChanges(IList<IUpdateEntry> entries)
            => _store.ExecuteTransaction(Check.NotNull(entries, nameof(entries)), _updateLogger);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override Task<int> SaveChangesAsync(
            IList<IUpdateEntry> entries,
            CancellationToken cancellationToken = default)
            => Task.FromResult(_store.ExecuteTransaction(Check.NotNull(entries, nameof(entries)), _updateLogger));

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool EnsureDatabaseCreated(StateManagerDependencies stateManagerDependencies)
            => _store.EnsureCreated(Check.NotNull(stateManagerDependencies, nameof(stateManagerDependencies)), _updateLogger);

     
    

 

      
    }
}
