using FileContextCore.Infrastructure;
using FileContextCore.Storage;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FileContextCore.Query.ExpressionVisitors
{
    class FileContextEntityQueryableExpressionVisitorFactory : IEntityQueryableExpressionVisitorFactory
    {
        private FileContextCache cache;

        public FileContextEntityQueryableExpressionVisitorFactory(FileContextCache _cache)
        {
            cache = _cache;
        }

        public ExpressionVisitor Create(EntityQueryModelVisitor queryModelVisitor, IQuerySource querySource) => new FileContextEntityQueryableExpressionVisitor(queryModelVisitor, querySource, cache);
    }
}
