// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using FileContextCore.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace FileContextCore.Infrastructure
{
    /// <summary>
    ///     <para>
    ///         Allows in-memory specific configuration to be performed on <see cref="DbContextOptions" />.
    ///     </para>
    ///     <para>
    ///         Instances of this class are returned from a call to
    ///         <see
    ///             cref="FileContextDbContextOptionsExtensions.UseFileContext(DbContextOptionsBuilder, string, string)" />
    ///         and it is not designed to be directly constructed in your application code.
    ///     </para>
    /// </summary>
    class FileContextDbContextOptionsBuilder
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FileContextDbContextOptionsBuilder" /> class.
        /// </summary>
        /// <param name="optionsBuilder"> The options builder. </param>
        public FileContextDbContextOptionsBuilder([NotNull] DbContextOptionsBuilder optionsBuilder)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));

            OptionsBuilder = optionsBuilder;
        }

        /// <summary>
        ///     Clones the configuration in this builder.
        /// </summary>
        /// <returns> The cloned configuration. </returns>
        protected virtual DbContextOptionsBuilder OptionsBuilder { get; }
    }
}
