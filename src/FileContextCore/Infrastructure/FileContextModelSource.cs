using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Internal;

namespace FileContextCore.Infrastructure
{
    class FileContextModelSource : ModelSource
    {
        public FileContextModelSource(IDbSetFinder setFinder, ICoreConventionSetBuilder coreConventionSetBuilder, IModelCustomizer modelCustomizer, IModelCacheKeyFactory modelCacheKeyFactory)
            : base(setFinder, coreConventionSetBuilder, modelCustomizer, modelCacheKeyFactory)
        {

        }
    }
}
