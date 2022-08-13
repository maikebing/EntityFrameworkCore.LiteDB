// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using JetBrains.Annotations;
using MaikeBing.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

namespace MaikeBing.EntityFrameworkCore.NoSqLiteDB.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class LiteDBTransactionManager : IDbContextTransactionManager, ITransactionEnlistmentManager
    {
        private static readonly LiteDBTransaction _stubTransaction = new LiteDBTransaction();

        private readonly IDiagnosticsLogger<DbLoggerCategory.Database.Transaction> _logger;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public LiteDBTransactionManager(
            [NotNull] IDiagnosticsLogger<DbLoggerCategory.Database.Transaction> logger)
        {
            Check.NotNull(logger, nameof(logger));

            _logger = logger;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IDbContextTransaction BeginTransaction()
        {
            _logger.TransactionIgnoredWarning();

            return _stubTransaction;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual Task<IDbContextTransaction> BeginTransactionAsync(
            CancellationToken cancellationToken = default)
        {
            _logger.TransactionIgnoredWarning();

            return Task.FromResult<IDbContextTransaction>(_stubTransaction);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void CommitTransaction() => _logger.TransactionIgnoredWarning();

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void RollbackTransaction() => _logger.TransactionIgnoredWarning();

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IDbContextTransaction CurrentTransaction => null;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual Transaction EnlistedTransaction => null;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void EnlistTransaction(Transaction transaction)
        {
            _logger.TransactionIgnoredWarning();
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void ResetState()
        {
        }

        public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            _logger.TransactionIgnoredWarning();
            return Task.CompletedTask;
        }

        public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            _logger.TransactionIgnoredWarning();
            return Task.CompletedTask;
        }

        public Task ResetStateAsync(CancellationToken cancellationToken = default)
        {
            _logger.TransactionIgnoredWarning();
            return Task.CompletedTask;
        }
    }
}
