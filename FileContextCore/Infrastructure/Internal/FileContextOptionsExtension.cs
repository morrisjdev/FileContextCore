// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;
using FileContextCore.Extensions;
using FileContextCore.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FileContextCore.Infrastructure.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    class FileContextOptionsExtension : IDbContextOptionsExtension
    {
        private string _serializer = "json";
        private string _filemanager = "default";
        private string _logFragment;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public FileContextOptionsExtension()
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected FileContextOptionsExtension([NotNull] FileContextOptionsExtension copyFrom)
        {
            _serializer = copyFrom._serializer;
            _filemanager = copyFrom._filemanager;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual FileContextOptionsExtension Clone() => new FileContextOptionsExtension(this);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual string Serializer => _serializer;

        public virtual string FileManager => _filemanager;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual FileContextOptionsExtension WithSerializerAndFileManager(string serializer, string filemanager)
        {
            FileContextOptionsExtension clone = Clone();

            clone._serializer = serializer;
            clone._filemanager = filemanager;

            return clone;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool ApplyServices(IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));

            services.AddEntityFrameworkFileContext();

            return true;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual long GetServiceProviderHashCode() => 0;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void Validate(IDbContextOptions options)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual string LogFragment
        {
            get
            {
                if (_logFragment == null)
                {
                    StringBuilder builder = new StringBuilder();

                    builder.Append("serializer=").Append(_serializer).Append(";filemanager=").Append(_filemanager).Append(' ');

                    _logFragment = builder.ToString();
                }

                return _logFragment;
            }
        }
    }
}
