using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using FileContextCore.ValueGeneration;
using FileContextCore.Query;
using FileContextCore.Query.ExpressionVisitors;
using FileContextCore.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FileContextCore.Storage
{
    class FileContextProviderServices : DatabaseProviderServices
    {
        public FileContextProviderServices(IServiceProvider services) : base(services)
        {

        }

        public override string InvariantName => "File Context";

        public override IDatabaseCreator Creator => GetService<FileContextCreator>();

        public override IDatabase Database => GetService<FileContextDatabase>();

        public override IEntityQueryableExpressionVisitorFactory EntityQueryableExpressionVisitorFactory => GetService<FileContextEntityQueryableExpressionVisitorFactory>();

        public override IEntityQueryModelVisitorFactory EntityQueryModelVisitorFactory => GetService<FileContextEntityQueryModelVisitorFactory>();

        public override IModelSource ModelSource => GetService<FileContextModelSource>();

        public override IQueryContextFactory QueryContextFactory => GetService<FileContextQueryContextFactory>();

        public override IDbContextTransactionManager TransactionManager => GetService<FileContextTransactionManager>();

        public override IValueGeneratorCache ValueGeneratorCache => GetService<FileValueGeneratorCache>();

        public override IValueGeneratorSelector ValueGeneratorSelector => GetService<FileContextValueGeneratorSelector>();
    }
}
