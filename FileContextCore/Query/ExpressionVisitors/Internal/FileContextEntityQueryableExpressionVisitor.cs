// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using FileContextCore.Query.Internal;
using FileContextCore.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Remotion.Linq.Clauses;

namespace FileContextCore.Query.ExpressionVisitors.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    class FileContextEntityQueryableExpressionVisitor : EntityQueryableExpressionVisitor
    {
        private readonly IModel _model;
        private readonly IMaterializerFactory _materializerFactory;
        private readonly IQuerySource _querySource;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public FileContextEntityQueryableExpressionVisitor(
            [NotNull] IModel model,
            [NotNull] IMaterializerFactory materializerFactory,
            [NotNull] EntityQueryModelVisitor entityQueryModelVisitor,
            [CanBeNull] IQuerySource querySource)
            : base(Check.NotNull(entityQueryModelVisitor, nameof(entityQueryModelVisitor)))
        {
            Check.NotNull(model, nameof(model));
            Check.NotNull(materializerFactory, nameof(materializerFactory));

            _model = model;
            _materializerFactory = materializerFactory;
            _querySource = querySource;
        }

        private new FileContextQueryModelVisitor QueryModelVisitor
            => (FileContextQueryModelVisitor)base.QueryModelVisitor;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override Expression VisitEntityQueryable(Type elementType)
        {
            Check.NotNull(elementType, nameof(elementType));

            IEntityType entityType = QueryModelVisitor.QueryCompilationContext.FindEntityType(_querySource)
                             ?? _model.FindEntityType(elementType);

            if (QueryModelVisitor.QueryCompilationContext
                .QuerySourceRequiresMaterialization(_querySource))
            {
                Expression<Func<IEntityType, Microsoft.EntityFrameworkCore.Storage.ValueBuffer, object>> materializer = _materializerFactory.CreateMaterializer(entityType);

                return Expression.Call(
                    FileContextQueryModelVisitor.EntityQueryMethodInfo.MakeGenericMethod(elementType),
                    EntityQueryModelVisitor.QueryContextParameter,
                    Expression.Constant(entityType),
                    Expression.Constant(entityType.FindPrimaryKey()),
                    materializer,
                    Expression.Constant(QueryModelVisitor.QueryCompilationContext.IsTrackingQuery));
            }

            return Expression.Call(
                FileContextQueryModelVisitor.ProjectionQueryMethodInfo,
                EntityQueryModelVisitor.QueryContextParameter,
                Expression.Constant(entityType));
        }
    }
}
