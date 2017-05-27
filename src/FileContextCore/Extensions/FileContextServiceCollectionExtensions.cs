using FileContextCore.Helper;
using FileContextCore.Infrastructure;
using FileContextCore.Query;
using FileContextCore.Query.ExpressionVisitors;
using FileContextCore.Storage;
using FileContextCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileContextCore.Extensions
{
    static class FileContextServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkFileContext(this IServiceCollection services)
        {
            services.AddEntityFramework();

            services.TryAddEnumerable(ServiceDescriptor
                .Singleton<IDatabaseProvider, DatabaseProvider<FileContextProviderServices, FileContextOptionsExtension>>());

            services.TryAdd(new ServiceCollection()
                .AddSingleton<FileContextModelSource>()
                .AddSingleton<FileValueGeneratorCache>()
                .AddScoped<FileContextValueGeneratorSelector>()
                .AddScoped<FileContextProviderServices>()
                .AddScoped<FileContextCreator>()
                .AddScoped<FileContextDatabase>()
                .AddScoped<FileContextEntityQueryableExpressionVisitorFactory>()
                .AddScoped<FileContextEntityQueryModelVisitorFactory>()
                .AddScoped<FileContextQueryContextFactory>()
                .AddScoped<FileContextTransactionManager>());

            QueryHelper.cache = new FileContextCache();

            return services;
        }
    }
}