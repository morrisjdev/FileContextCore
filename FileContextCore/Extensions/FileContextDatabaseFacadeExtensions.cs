// Copyright (c) morrisjdev. All rights reserved.
// Original copyright (c) .NET Foundation. All rights reserved.
// Modified version by morrisjdev
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using FileContextCore.Infrastructure.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

// ReSharper disable once CheckNamespace
namespace FileContextCore
{
    /// <summary>
    ///     In-memory specific extension methods for <see cref="DbContext.Database" />.
    /// </summary>
    public static class FileContextDatabaseFacadeExtensions
    {
        /// <summary>
        ///     <para>
        ///         Returns <c>true</c> if the database provider currently in use is the in-memory provider.
        ///     </para>
        ///     <para>
        ///         This method can only be used after the <see cref="DbContext" /> has been configured because
        ///         it is only then that the provider is known. This means that this method cannot be used
        ///         in <see cref="DbContext.OnConfiguring" /> because this is where application code sets the
        ///         provider to use as part of configuring the context.
        ///     </para>
        /// </summary>
        /// <param name="database"> The facade from <see cref="DbContext.Database" />. </param>
        /// <returns> <c>true</c> if the in-memory database is being used. </returns>
        public static bool IsFileContext([NotNull] this DatabaseFacade database)
            => database.ProviderName.Equals(
                typeof(FileContextOptionsExtension).GetTypeInfo().Assembly.GetName().Name,
                StringComparison.Ordinal);
    }
}
