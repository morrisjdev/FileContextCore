using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileContextCore.Storage
{
    class FileContextTransactionManager : IDbContextTransactionManager
    {
        private static FileContextTransaction _transaction = new FileContextTransaction();

        public IDbContextTransaction CurrentTransaction => null;

        public IDbContextTransaction BeginTransaction()
        {
            return _transaction;
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(BeginTransaction());
        }

        public void CommitTransaction()
        {

        }

        public void RollbackTransaction()
        {
            
        }
    }
}
