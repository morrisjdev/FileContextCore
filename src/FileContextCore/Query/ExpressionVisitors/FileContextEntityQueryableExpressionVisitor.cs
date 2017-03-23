using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using System.Linq.Expressions;
using FileContextCore.Serializer;
using FileContextCore.Infrastructure;
using FileContextCore.Storage;
using Remotion.Linq.Clauses.Expressions;

namespace FileContextCore.Query.ExpressionVisitors
{
    class FileContextEntityQueryableExpressionVisitor : EntityQueryableExpressionVisitor
    {
        private EntityQueryModelVisitor queryModelVisitor;
        private IQuerySource querySource;
        private FileContextCache cache;

        public FileContextEntityQueryableExpressionVisitor(EntityQueryModelVisitor _queryModelVisitor, IQuerySource _querySource, FileContextCache _cache) : base(_queryModelVisitor)
        {
            queryModelVisitor = _queryModelVisitor;
            querySource = _querySource;
            cache = _cache;
        }

        protected override Expression VisitEntityQueryable(Type elementType)
        {
            return Expression.Constant(cache.GetValues(elementType));
        }
    }
}
