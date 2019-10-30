// Copyright (c) morrisjdev & .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace FileContextCore.Query.Internal
{
    public partial class FileContextShapedQueryCompilingExpressionVisitor : ShapedQueryCompilingExpressionVisitor
    {
        private readonly Type _contextType;
        private readonly IDiagnosticsLogger<DbLoggerCategory.Query> _logger;

        public FileContextShapedQueryCompilingExpressionVisitor(
            ShapedQueryCompilingExpressionVisitorDependencies dependencies,
            QueryCompilationContext queryCompilationContext)
            : base(dependencies, queryCompilationContext)
        {
            _contextType = queryCompilationContext.ContextType;
            _logger = queryCompilationContext.Logger;
        }

        protected override Expression VisitExtension(Expression extensionExpression)
        {
            switch (extensionExpression)
            {
                case FileContextQueryExpression inMemoryQueryExpression:
                    inMemoryQueryExpression.ApplyProjection();
                    return Visit(inMemoryQueryExpression.ServerQueryExpression);

                case FileContextTableExpression inMemoryTableExpression:
                    return Expression.Call(
                        _tableMethodInfo,
                        QueryCompilationContext.QueryContextParameter,
                        Expression.Constant(inMemoryTableExpression.EntityType));
            }

            return base.VisitExtension(extensionExpression);
        }

        protected override Expression VisitShapedQueryExpression(ShapedQueryExpression shapedQueryExpression)
        {
            var inMemoryQueryExpression = (FileContextQueryExpression)shapedQueryExpression.QueryExpression;

            var shaper = new ShaperExpressionProcessingExpressionVisitor(
                inMemoryQueryExpression, inMemoryQueryExpression.CurrentParameter)
                .Inject(shapedQueryExpression.ShaperExpression);

            shaper = InjectEntityMaterializers(shaper);

            var innerEnumerable = Visit(inMemoryQueryExpression);

            shaper = new FileContextProjectionBindingRemovingExpressionVisitor().Visit(shaper);

            shaper = new CustomShaperCompilingExpressionVisitor(IsTracking).Visit(shaper);

            var shaperLambda = (LambdaExpression)shaper;

            return Expression.New(
                typeof(QueryingEnumerable<>).MakeGenericType(shaperLambda.ReturnType).GetConstructors()[0],
                QueryCompilationContext.QueryContextParameter,
                innerEnumerable,
                Expression.Constant(shaperLambda.Compile()),
                Expression.Constant(_contextType),
                Expression.Constant(_logger));
        }

        private static readonly MethodInfo _tableMethodInfo
            = typeof(FileContextShapedQueryCompilingExpressionVisitor).GetTypeInfo()
                .GetDeclaredMethod(nameof(Table));

        private static IEnumerable<ValueBuffer> Table(
            QueryContext queryContext,
            IEntityType entityType)
        {
            return ((FileContextQueryContext)queryContext).Store
                .GetTables(entityType)
                .SelectMany(t => t.Rows.Select(vs => new ValueBuffer(vs)));
        }
    }
}
