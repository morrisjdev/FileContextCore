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

namespace FileContextCore.Query
{
    class FileContextEntityQueryModelVisitor : EntityQueryModelVisitor
    {
        private FileContextCache cache;

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
            QueryCompilationContext queryCompilationContext,
            FileContextCache _cache)
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
            cache = _cache;
        }

        protected override void IncludeNavigations(IncludeSpecification includeSpecification, Type resultType, Expression accessorExpression, bool querySourceRequiresTracking)
        {
            SelectorIncludeInjectingExpressionVisitor includeExpressionVisitor
               = new SelectorIncludeInjectingExpressionVisitor(
                    includeSpecification,
                    accessorExpression,
                    querySourceRequiresTracking,
                    cache);

            Expression = includeExpressionVisitor.Visit(Expression);
        }

        private class SelectorIncludeInjectingExpressionVisitor : ExpressionVisitorBase
        {
            private readonly IncludeSpecification includeSpecification;
            private readonly Expression accessorExpression;
            private readonly bool querySourceRequiresTracking;
            private FileContextCache cache;

            public SelectorIncludeInjectingExpressionVisitor(IncludeSpecification _includeSpecification, Expression _accessorExpression, bool _querySourceRequiresTracking, FileContextCache _cache)
            {
                includeSpecification = _includeSpecification;
                accessorExpression = _accessorExpression;
                querySourceRequiresTracking = _querySourceRequiresTracking;
                cache = _cache;
            }

            public override Expression Visit(Expression node)
            {
                IEnumerable values = (IEnumerable)((ConstantExpression)node).Value;

                foreach (INavigation nav in includeSpecification.NavigationPath)
                {
                    IEntityType t;
                    IClrPropertyGetter[] objProps;
                    IClrPropertyGetter[] refObjProps;

                    if (nav.IsDependentToPrincipal())
                    {
                        t = nav.ForeignKey.PrincipalEntityType;

                        objProps = nav.ForeignKey.Properties.Select(x => x.GetGetter()).ToArray();
                        refObjProps = nav.ForeignKey.PrincipalKey.Properties.Select(x => x.GetGetter()).ToArray();
                    }
                    else
                    {
                        t = nav.ForeignKey.DeclaringEntityType;

                        objProps = nav.ForeignKey.PrincipalKey.Properties.Select(x => x.GetGetter()).ToArray();
                        refObjProps = nav.ForeignKey.Properties.Select(x => x.GetGetter()).ToArray();
                    }

                    if(objProps.Count() != refObjProps.Count())
                    {
                        return Expression.Constant(values);
                    }

                    IClrPropertySetter valueSetter = nav.GetSetter();

                    if (nav.IsCollection())
                    {
                        foreach (object obj in values)
                        {
                            object[] objValues = objProps.Select(x => x.GetClrValue(obj)).ToArray();

                            valueSetter.SetClrValue(obj, cache.Filter(t.ClrType, x => {

                                int result = 0;

                                object[] refValues = refObjProps.Select(y => y.GetClrValue(x)).ToArray();

                                if(refValues.Count() == objValues.Count())
                                {

                                    for (int i = 0; i < refValues.Count(); i++)
                                    {
                                        if(Equals(objValues[i], refValues[i]))
                                        {
                                            result++;
                                        }
                                    }
                                }

                                return result == refValues.Count();
                            }));
                        }
                    }
                    else
                    {
                        foreach (object obj in values)
                        {
                            object[] objValues = objProps.Select(x => x.GetClrValue(obj)).ToArray();

                            IList matching = cache.Filter(t.ClrType, x => {

                                int result = 0;

                                object[] refValues = refObjProps.Select(y => y.GetClrValue(x)).ToArray();

                                if (refValues.Count() == objValues.Count())
                                {

                                    for (int i = 0; i < refValues.Count(); i++)
                                    {
                                        if (Equals(objValues[i], refValues[i]))
                                        {
                                            result++;
                                        }
                                    }
                                }

                                return result == refValues.Count();
                            });

                            if (matching.Count > 0)
                            {
                                valueSetter.SetClrValue(obj, matching[0]);
                            }
                        }
                    }
                }

                return Expression.Constant(values);
            }
        }
    }
}
