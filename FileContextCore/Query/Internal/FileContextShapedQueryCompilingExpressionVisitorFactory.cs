// Copyright (c) morrisjdev & .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Query;

namespace FileContextCore.Query.Internal
{
    public class FileContextShapedQueryCompilingExpressionVisitorFactory : IShapedQueryCompilingExpressionVisitorFactory
    {
        private readonly ShapedQueryCompilingExpressionVisitorDependencies _dependencies;

        public FileContextShapedQueryCompilingExpressionVisitorFactory(ShapedQueryCompilingExpressionVisitorDependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public virtual ShapedQueryCompilingExpressionVisitor Create(QueryCompilationContext queryCompilationContext)
            => new FileContextShapedQueryCompilingExpressionVisitor(_dependencies, queryCompilationContext);
    }

}
