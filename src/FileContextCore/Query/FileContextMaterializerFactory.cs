using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;

namespace FileContextCore.Query
{
    class FileContextMaterializerFactory : IMaterializerFactory
    {
        private IEntityMaterializerSource entityMaterializerSource;

        public FileContextMaterializerFactory(IEntityMaterializerSource _entityMaterializerSource)
        {
            entityMaterializerSource = _entityMaterializerSource;
        }

        public virtual Expression<Func<IEntityType, ValueBuffer, object>> CreateMaterializer(IEntityType entityType)
        {
            ParameterExpression entityTypeParameter = Expression.Parameter(typeof(IEntityType), "entityType");

            ParameterExpression valueBufferParameter = Expression.Parameter(typeof(ValueBuffer), "valueBuffer");

            List<IEntityType> concreteEntityTypes = entityType.GetConcreteTypesInHierarchy().ToList();

            if (concreteEntityTypes.Count == 1)
            {
                return Expression.Lambda<Func<IEntityType, ValueBuffer, object>>(
                    entityMaterializerSource
                        .CreateMaterializeExpression(
                            concreteEntityTypes[0], valueBufferParameter),
                    entityTypeParameter,
                    valueBufferParameter);
            }

            LabelTarget returnLabelTarget = Expression.Label(typeof(object));

            Expression[] blockExpressions = new Expression[]
                {
                    Expression.IfThen(
                        Expression.Equal(
                            entityTypeParameter,
                            Expression.Constant(concreteEntityTypes[0])),
                        Expression.Return(
                            returnLabelTarget,
                            entityMaterializerSource
                                .CreateMaterializeExpression(
                                    concreteEntityTypes[0], valueBufferParameter))),
                    Expression.Label(
                        returnLabelTarget,
                        Expression.Default(returnLabelTarget.Type))
                };

            foreach (var concreteEntityType in concreteEntityTypes.Skip(1))
            {
                blockExpressions[0]
                    = Expression.IfThenElse(
                        Expression.Equal(
                            entityTypeParameter,
                            Expression.Constant(concreteEntityType)),
                        Expression.Return(
                            returnLabelTarget,
                            entityMaterializerSource
                                .CreateMaterializeExpression(concreteEntityType, valueBufferParameter)),
                        blockExpressions[0]);
            }

            return Expression.Lambda<Func<IEntityType, ValueBuffer, object>>(
                Expression.Block(blockExpressions),
                entityTypeParameter,
                valueBufferParameter);
        }
    }
}