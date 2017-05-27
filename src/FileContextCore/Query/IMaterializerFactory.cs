using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace FileContextCore.Query
{
    interface IMaterializerFactory
    {
        Expression<Func<IEntityType, ValueBuffer, object>> CreateMaterializer(IEntityType entityType);
    }
}
