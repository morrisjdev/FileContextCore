using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Update;
using System.Threading;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using System.Dynamic;
using FileContextCore.Serializer;
using FileContextCore.Helper;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using FileContextCore.Infrastructure;
using System.Collections;
using System.Reflection;
using Remotion.Linq;
using FileContextCore.Query;

namespace FileContextCore.Storage
{
    class FileContextDatabase : Database
    {
        public FileContextDatabase(IQueryCompilationContextFactory queryCompilationContextFactory) : base(queryCompilationContextFactory)
        {
        }

        public override int SaveChanges(IReadOnlyList<IUpdateEntry> entries)
        {
            return QueryHelper.cache.ExecuteTransaction(entries);
        }

        public override Task<int> SaveChangesAsync(IReadOnlyList<IUpdateEntry> entries, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.FromResult(QueryHelper.cache.ExecuteTransaction(entries));
        }
    }
}
