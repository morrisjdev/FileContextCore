// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace FileContextCore.Storage.Internal
{
    class FileContextTransaction : IDbContextTransaction
    {
        public virtual Guid TransactionId { get; } = Guid.NewGuid();

        public virtual void Commit()
        {
        }

        public virtual void Rollback()
        {
        }

        public virtual void Dispose()
        {
        }
    }
}
