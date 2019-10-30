// Copyright (c) morrisjdev & .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace FileContextCore.Query.Internal
{
    public class FileContextTableExpression : Expression, IPrintableExpression
    {
        public FileContextTableExpression(IEntityType entityType)
        {
            EntityType = entityType;
        }

        public override Type Type => typeof(IEnumerable<ValueBuffer>);

        public virtual IEntityType EntityType { get; }

        public sealed override ExpressionType NodeType => ExpressionType.Extension;

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            return this;
        }

        public virtual void Print(ExpressionPrinter expressionPrinter)
        {
            expressionPrinter.Append(nameof(FileContextTableExpression) + ": Entity: " + EntityType.DisplayName());
        }
    }
}
