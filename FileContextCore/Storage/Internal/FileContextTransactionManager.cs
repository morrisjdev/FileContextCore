// Copyright (c) morrisjdev. All rights reserved.
// Original copyright (c) .NET Foundation. All rights reserved.
// Modified version by morrisjdev
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using FileContextCore.Internal;
using FileContextCore.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace FileContextCore.Storage.Internal
{
    /// <summary>
    ///     <para>
    ///         This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///         the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///         any release. You should only use it directly in your code with extreme caution and knowing that
    ///         doing so can result in application failures when updating to a new Entity Framework Core release.
    ///     </para>
    ///     <para>
    ///         The service lifetime is <see cref="ServiceLifetime.Scoped"/>. This means that each
    ///         <see cref="DbContext"/> instance will use its own instance of this service.
    ///         The implementation may depend on other services registered with any lifetime.
    ///         The implementation does not need to be thread-safe.
    ///     </para>
    /// </summary>
    public class FileContextTransactionManager : IDbContextTransactionManager, ITransactionEnlistmentManager
    {
        private static readonly FileContextTransaction _stubTransaction = new FileContextTransaction();

        private readonly IDiagnosticsLogger<DbLoggerCategory.Database.Transaction> _logger;

    
        public FileContextTransactionManager(
            [NotNull] IDiagnosticsLogger<DbLoggerCategory.Database.Transaction> logger)
        {
            Check.NotNull(logger, nameof(logger));

            _logger = logger;
        }

    
        public virtual IDbContextTransaction BeginTransaction()
        {
            _logger.TransactionIgnoredWarning();

            return _stubTransaction;
        }

    
        public virtual Task<IDbContextTransaction> BeginTransactionAsync(
            CancellationToken cancellationToken = default)
        {
            _logger.TransactionIgnoredWarning();

            return Task.FromResult<IDbContextTransaction>(_stubTransaction);
        }

    
        public virtual void CommitTransaction() => _logger.TransactionIgnoredWarning();

    
        public virtual void RollbackTransaction() => _logger.TransactionIgnoredWarning();

    
        public virtual IDbContextTransaction CurrentTransaction => null;

    
        public virtual Transaction EnlistedTransaction => null;

    
        public virtual void EnlistTransaction(Transaction transaction)
        {
            _logger.TransactionIgnoredWarning();
        }

    
        public virtual void ResetState()
        {
        }

    
        /// <param name="cancellationToken"> A <see cref="CancellationToken" /> to observe while waiting for the task to complete. </param>
        public virtual Task ResetStateAsync(CancellationToken cancellationToken = default)
        {
            ResetState();

            return default;
        }
    }
}
