using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remotion.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using FileContextCore.Storage;
using Remotion.Linq.Clauses;
using FileContextCore.Helper;

namespace FileContextCore.Query
{
    class FileContextEntityQueryModelVisitor : EntityQueryModelVisitor
    {
        public FileContextEntityQueryModelVisitor(
            IQueryOptimizer queryOptimizer,
            INavigationRewritingExpressionVisitorFactory navigationRewritingExpressionVisitorFactory,
            ISubQueryMemberPushDownExpressionVisitor subQueryMemberPushDownExpressionVisitor,
            IQuerySourceTracingExpressionVisitorFactory querySourceTracingExpressionVisitorFactory,
            IEntityResultFindingExpressionVisitorFactory entityResultFindingExpressionVisitorFactory,
            ITaskBlockingExpressionVisitor taskBlockingExpressionVisitor,
            IMemberAccessBindingExpressionVisitorFactory memberAccessBindingExpressionVisitorFactory,
            IOrderingExpressionVisitorFactory orderingExpressionVisitorFactory,
            IProjectionExpressionVisitorFactory projectionExpressionVisitorFactory,
            IEntityQueryableExpressionVisitorFactory entityQueryableExpressionVisitorFactory,
            IQueryAnnotationExtractor queryAnnotationExtractor,
            IResultOperatorHandler resultOperatorHandler,
            IEntityMaterializerSource entityMaterializerSource,
            IExpressionPrinter expressionPrinter,
            QueryCompilationContext queryCompilationContext)
            : base(queryOptimizer,
                navigationRewritingExpressionVisitorFactory,
                subQueryMemberPushDownExpressionVisitor,
                querySourceTracingExpressionVisitorFactory,
                entityResultFindingExpressionVisitorFactory,
                taskBlockingExpressionVisitor,
                memberAccessBindingExpressionVisitorFactory,
                orderingExpressionVisitorFactory,
                projectionExpressionVisitorFactory,
                entityQueryableExpressionVisitorFactory,
                queryAnnotationExtractor,
                resultOperatorHandler,
                entityMaterializerSource,
                expressionPrinter,
                queryCompilationContext)
        {
        }

        protected override void IncludeNavigations(IncludeSpecification includeSpecification, Type resultType, Expression accessorExpression, bool querySourceRequiresTracking)
        {
            SelectorIncludeInjectingExpressionVisitor includeExpressionVisitor
               = new SelectorIncludeInjectingExpressionVisitor(
                    includeSpecification,
                    accessorExpression,
                    querySourceRequiresTracking);

            Expression = includeExpressionVisitor.Visit(Expression);
        }

        private class SelectorIncludeInjectingExpressionVisitor : ExpressionVisitorBase
        {
            private readonly IncludeSpecification includeSpecification;
            private readonly Expression accessorExpression;
            private readonly bool querySourceRequiresTracking;

            public SelectorIncludeInjectingExpressionVisitor(IncludeSpecification _includeSpecification, Expression _accessorExpression, bool _querySourceRequiresTracking)
            {
                includeSpecification = _includeSpecification;
                accessorExpression = _accessorExpression;
                querySourceRequiresTracking = _querySourceRequiresTracking;
            }

            public override Expression Visit(Expression node)
            {
                MethodCallExpression expr = (MethodCallExpression)node;
                Type t = expr.Type.GenericTypeArguments[0];
                MethodInfo info = typeof(QueryHelper).GetMethod(nameof(QueryHelper.LoadRelatedData)).MakeGenericMethod(t);

                return Expression.Call(info, node, Expression.Constant(includeSpecification));
            }
        }
    }
}
