// Copyright (c) morrisjdev. All rights reserved.
// Original copyright (c) .NET Foundation. All rights reserved.
// Modified version by morrisjdev
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace FileContextCore.Storage.Internal
{

    public class FileContextTransaction : IDbContextTransaction
    {
    
        public virtual Guid TransactionId { get; } = Guid.NewGuid();

    
        public virtual void Commit()
        {
        }

    
        public virtual void Rollback()
        {
        }

    
        public virtual Task CommitAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;

    
        public virtual Task RollbackAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;

    
        public virtual void Dispose()
        {
        }

    
        public virtual ValueTask DisposeAsync()
            => default;
    }
}
