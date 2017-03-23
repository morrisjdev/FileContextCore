using FileContextCore.Extensions;
using FileContextCore.FileManager;
using FileContextCore.Serializer;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileContextCore.Infrastructure
{
    class FileContextOptionsExtension : IDbContextOptionsExtension
    {
        public void ApplyServices(IServiceCollection builder)
        {
            builder.AddEntityFrameworkFileContext();
        }
    }
}
