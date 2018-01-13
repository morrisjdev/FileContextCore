// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using FileContextCore.Extensions.Internal;
using FileContextCore.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;

namespace FileContextCore.Storage.Internal
{
    class FileContextTransactionManager : IDbContextTransactionManager
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
            CancellationToken cancellationToken = default(CancellationToken))
        {
            _logger.TransactionIgnoredWarning();

            return Task.FromResult<IDbContextTransaction>(_stubTransaction);
        }

        public virtual void CommitTransaction() => _logger.TransactionIgnoredWarning();

        public virtual void RollbackTransaction() => _logger.TransactionIgnoredWarning();

        public virtual IDbContextTransaction CurrentTransaction => null;

        public virtual void ResetState()
        {
        }
    }
}
