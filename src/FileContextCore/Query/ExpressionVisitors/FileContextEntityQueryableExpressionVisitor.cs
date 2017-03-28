using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using FileContextCore.Serializer;
using FileContextCore.Infrastructure;
using FileContextCore.Storage;
using Remotion.Linq.Clauses.Expressions;
using FileContextCore.Helper;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Collections;
using System.Linq.Expressions;
using Remotion.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace FileContextCore.Query.ExpressionVisitors
{
    class FileContextEntityQueryableExpressionVisitor : EntityQueryableExpressionVisitor
    {
        private EntityQueryModelVisitor queryModelVisitor;
        private IQuerySource querySource;

        public FileContextEntityQueryableExpressionVisitor(EntityQueryModelVisitor _queryModelVisitor, IQuerySource _querySource) : base(_queryModelVisitor)
        {
            queryModelVisitor = _queryModelVisitor;
            querySource = _querySource;
        }

        protected override Expression VisitEntityQueryable(Type elementType)
        {
            MethodInfo info = typeof(QueryHelper).GetMethod(nameof(QueryHelper.GetValues)).MakeGenericMethod(elementType);
            return Expression.Call(info);
        }
    }
}
