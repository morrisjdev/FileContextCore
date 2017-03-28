using FileContextCore.Storage;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileContextCore.Query
{
    class FileContextQueryContextFactory : QueryContextFactory
    {
        public FileContextQueryContextFactory(IStateManager stateManager, ICurrentDbContext context, IConcurrencyDetector concurrencyDetector, IChangeDetector changeDetector)
            : base(context, concurrencyDetector)
        {
        }

        public override QueryContext Create()
        {
            return new QueryContext(CreateQueryBuffer, StateManager, ConcurrencyDetector);
        }
    }
}
