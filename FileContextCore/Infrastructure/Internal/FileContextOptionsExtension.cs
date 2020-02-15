// Copyright (c) morrisjdev. All rights reserved.
// Original copyright (c) .NET Foundation. All rights reserved.
// Modified version by morrisjdev
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FileContextCore.Storage;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace FileContextCore.Infrastructure.Internal
{

    public class FileContextOptionsExtension : IDbContextOptionsExtension
    {
        private FileContextDatabaseRoot _databaseRoot;
        private DbContextOptionsExtensionInfo _info;
        private FileContextScopedOptions _options;

    
        public FileContextOptionsExtension()
        {
            _options = new FileContextScopedOptions(null, null, null, null);
        }

    
        protected FileContextOptionsExtension([NotNull] FileContextOptionsExtension copyFrom)
        {
            _options = (FileContextScopedOptions)copyFrom._options.Clone();
            _databaseRoot = copyFrom._databaseRoot;
        }

    
        public virtual DbContextOptionsExtensionInfo Info
            => _info ??= new ExtensionInfo(this);

    
        protected virtual FileContextOptionsExtension Clone() => new FileContextOptionsExtension(this);

    
        public virtual FileContextScopedOptions Options => _options;

    
        public virtual FileContextOptionsExtension WithCustomOptions(string databaseName, string serializer, string fileManager, string location)
        {
            var clone = Clone();
            clone._options.Location = location;
            clone._options.DatabaseName = databaseName;
            clone._options.FileManager = fileManager;
            clone._options.Serializer = serializer;

            return clone;
        }

    
        public virtual FileContextDatabaseRoot DatabaseRoot => _databaseRoot;

    
        public virtual FileContextOptionsExtension WithDatabaseRoot([NotNull] FileContextDatabaseRoot databaseRoot)
        {
            var clone = Clone();

            clone._databaseRoot = databaseRoot;

            return clone;
        }

    
        public virtual void ApplyServices(IServiceCollection services)
        {
            services.AddEntityFrameworkFileContextDatabase();
        }

    
        public virtual void Validate(IDbContextOptions options)
        {
        }

        private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
        {
            private string _logFragment;

            public ExtensionInfo(IDbContextOptionsExtension extension)
                : base(extension)
            {
            }

            private new FileContextOptionsExtension Extension
                => (FileContextOptionsExtension)base.Extension;

            public override bool IsDatabaseProvider => true;

            public override string LogFragment
            {
                get
                {
                    if (_logFragment == null)
                    {
                        var builder = new StringBuilder();

                        builder.Append("Location=").Append(Extension.Options.Location).Append(' ');
                        builder.Append("DatabaseName=").Append(Extension.Options.DatabaseName).Append(' ');
                        builder.Append("Serializer=").Append(Extension.Options.Serializer).Append(' ');
                        builder.Append("FileManager=").Append(Extension.Options.FileManager).Append(' ');

                        _logFragment = builder.ToString();
                    }

                    return _logFragment;
                }
            }

            public override long GetServiceProviderHashCode() => Extension._databaseRoot?.GetHashCode() ?? 0L;

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
                => debugInfo["FileContextDatabase:DatabaseRoot"]
                    = (Extension._databaseRoot?.GetHashCode() ?? 0L).ToString(CultureInfo.InvariantCulture);
        }
    }
}
