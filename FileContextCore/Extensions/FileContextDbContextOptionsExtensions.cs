// Copyright (c) morrisjdev. All rights reserved.
// Original copyright (c) .NET Foundation. All rights reserved.
// Modified version by morrisjdev
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using FileContextCore.Diagnostics;
using FileContextCore.FileManager;
using FileContextCore.Infrastructure;
using FileContextCore.Infrastructure.Internal;
using FileContextCore.Serializer;
using FileContextCore.Storage;
using FileContextCore.StoreManager;
using FileContextCore.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

// ReSharper disable once CheckNamespace
namespace FileContextCore
{
    /// <summary>
    ///     FileContextCore specific extension methods for <see cref="DbContextOptionsBuilder" />.
    /// </summary>
    public static class FileContextDbContextOptionsExtensions
    {
        #region NothingConfiguredUseJsonAndDefaultFileManager

        /// <summary>
        ///     Configures the context to connect to an in-memory database.
        ///     The in-memory database is shared anywhere the same name is used, but only for a given
        ///     service provider.
        /// </summary>
        /// <typeparam name="TContext"> The type of context being configured. </typeparam>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="databaseName">
        ///     The name of the in-memory database. This allows the scope of the in-memory database to be controlled
        ///     independently of the context. The in-memory database is shared anywhere the same name is used.
        /// </param>
        /// <param name="location">An optional parameter to define the location where the files are stored</param>
        /// <param name="password">An optional parameter to define a password if needed by store manager, serializer or file manager</param>
        /// <param name="databaseRoot">
        ///     All in-memory databases will be rooted in this object, allowing the application
        ///     to control their lifetime. This is useful when sometimes the context instance
        ///     is created explicitly with <c>new</c> while at other times it is resolved using dependency injection.
        /// </param>
        /// <param name="inMemoryOptionsAction">An optional action to allow additional in-memory specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder<TContext> UseFileContextDatabase<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            string databaseName = "",
            string location = null,
            string password = null,
            [CanBeNull] FileContextDatabaseRoot databaseRoot = null,
            [CanBeNull] Action<FileContextDbContextOptionsBuilder> inMemoryOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>) UseFileContextDatabase(
                (DbContextOptionsBuilder) optionsBuilder, databaseName, location, password, databaseRoot,
                inMemoryOptionsAction);

        /// <summary>
        ///     Configures the context to connect to a named in-memory database.
        ///     The in-memory database is shared anywhere the same name is used, but only for a given
        ///     service provider.
        /// </summary>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="databaseName">
        ///     The name of the in-memory database. This allows the scope of the in-memory database to be controlled
        ///     independently of the context. The in-memory database is shared anywhere the same name is used.
        /// </param>
        /// <param name="location">An optional parameter to define the location where the files are stored</param>
        /// <param name="password">An optional parameter to define a password if needed by store manager, serializer or file manager</param>
        /// <param name="databaseRoot">
        ///     All in-memory databases will be rooted in this object, allowing the application
        ///     to control their lifetime. This is useful when sometimes the context instance
        ///     is created explicitly with <c>new</c> while at other times it is resolved using dependency injection.
        /// </param>
        /// <param name="inMemoryOptionsAction">An optional action to allow additional in-memory specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder UseFileContextDatabase(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            string databaseName = "",
            string location = null,
            string password = null,
            [CanBeNull] FileContextDatabaseRoot databaseRoot = null,
            [CanBeNull] Action<FileContextDbContextOptionsBuilder> inMemoryOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));

            var extension = optionsBuilder.Options.FindExtension<FileContextOptionsExtension>()
                            ?? new FileContextOptionsExtension();

            extension = extension.WithCustomOptions(databaseName, location, password,
                typeof(DefaultStoreManager<JSONSerializer, DefaultFileManager>));

            if (databaseRoot != null)
            {
                extension = extension.WithDatabaseRoot(databaseRoot);
            }

            ConfigureWarnings(optionsBuilder);

            ((IDbContextOptionsBuilderInfrastructure) optionsBuilder).AddOrUpdateExtension(extension);

            inMemoryOptionsAction?.Invoke(new FileContextDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        #endregion

        #region UseDefaultStoreManagerWithSerializerAndFileManager

        /// <summary>
        ///     Configures the context to connect to an in-memory database.
        ///     The in-memory database is shared anywhere the same name is used, but only for a given
        ///     service provider.
        /// </summary>
        /// <typeparam name="TContext"> The type of context being configured. </typeparam>
        /// <typeparam name="TSerializer"> The type of the serializer to be used. </typeparam>
        /// <typeparam name="TFileManager"> The type of the file manager to be used. </typeparam>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="databaseName">
        ///     The name of the in-memory database. This allows the scope of the in-memory database to be controlled
        ///     independently of the context. The in-memory database is shared anywhere the same name is used.
        /// </param>
        /// <param name="location">An optional parameter to define the location where the files are stored</param>
        /// <param name="password">An optional parameter to define a password if needed by store manager, serializer or file manager</param>
        /// <param name="databaseRoot">
        ///     All in-memory databases will be rooted in this object, allowing the application
        ///     to control their lifetime. This is useful when sometimes the context instance
        ///     is created explicitly with <c>new</c> while at other times it is resolved using dependency injection.
        /// </param>
        /// <param name="inMemoryOptionsAction">An optional action to allow additional in-memory specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder<TContext> UseFileContextDatabase<TContext, TSerializer, TFileManager>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            string databaseName = "",
            string location = null,
            string password = null,
            [CanBeNull] FileContextDatabaseRoot databaseRoot = null,
            [CanBeNull] Action<FileContextDbContextOptionsBuilder> inMemoryOptionsAction = null)
            where TContext : DbContext
            where TSerializer : ISerializer
            where TFileManager : IFileManager
            => (DbContextOptionsBuilder<TContext>) UseFileContextDatabase<TSerializer, TFileManager>(
                optionsBuilder, databaseName, location, password, databaseRoot, inMemoryOptionsAction);

        /// <summary>
        ///     Configures the context to connect to a named in-memory database.
        ///     The in-memory database is shared anywhere the same name is used, but only for a given
        ///     service provider.
        /// </summary>
        /// <typeparam name="TSerializer"> The type of the serializer to be used. </typeparam>
        /// <typeparam name="TFileManager"> The type of the file manager to be used. </typeparam>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="databaseName">
        ///     The name of the in-memory database. This allows the scope of the in-memory database to be controlled
        ///     independently of the context. The in-memory database is shared anywhere the same name is used.
        /// </param>
        /// <param name="location">An optional parameter to define the location where the files are stored</param>
        /// <param name="password">An optional parameter to define a password if needed by store manager, serializer or file manager</param>
        /// <param name="databaseRoot">
        ///     All in-memory databases will be rooted in this object, allowing the application
        ///     to control their lifetime. This is useful when sometimes the context instance
        ///     is created explicitly with <c>new</c> while at other times it is resolved using dependency injection.
        /// </param>
        /// <param name="inMemoryOptionsAction">An optional action to allow additional in-memory specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder UseFileContextDatabase<TSerializer, TFileManager>(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            string databaseName = "",
            string location = null,
            string password = null,
            [CanBeNull] FileContextDatabaseRoot databaseRoot = null,
            [CanBeNull] Action<FileContextDbContextOptionsBuilder> inMemoryOptionsAction = null)
            where TSerializer : ISerializer
            where TFileManager : IFileManager
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));

            var extension = optionsBuilder.Options.FindExtension<FileContextOptionsExtension>()
                            ?? new FileContextOptionsExtension();

            extension = extension.WithCustomOptions(databaseName, location, password,
                typeof(DefaultStoreManager<TSerializer, TFileManager>));

            if (databaseRoot != null)
            {
                extension = extension.WithDatabaseRoot(databaseRoot);
            }

            ConfigureWarnings(optionsBuilder);

            ((IDbContextOptionsBuilderInfrastructure) optionsBuilder).AddOrUpdateExtension(extension);

            inMemoryOptionsAction?.Invoke(new FileContextDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        #endregion

        #region UseCustomStoreManager

        /// <summary>
        ///     Configures the context to connect to an in-memory database.
        ///     The in-memory database is shared anywhere the same name is used, but only for a given
        ///     service provider.
        /// </summary>
        /// <typeparam name="TContext"> The type of context being configured. </typeparam>
        /// <typeparam name="TStoreManager"> The type of the store manager to be used. </typeparam>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="databaseName">
        ///     The name of the in-memory database. This allows the scope of the in-memory database to be controlled
        ///     independently of the context. The in-memory database is shared anywhere the same name is used.
        /// </param>
        /// <param name="location">An optional parameter to define the location where the files are stored</param>
        /// <param name="password">An optional parameter to define a password if needed by store manager, serializer or file manager</param>
        /// <param name="databaseRoot">
        ///     All in-memory databases will be rooted in this object, allowing the application
        ///     to control their lifetime. This is useful when sometimes the context instance
        ///     is created explicitly with <c>new</c> while at other times it is resolved using dependency injection.
        /// </param>
        /// <param name="inMemoryOptionsAction">An optional action to allow additional in-memory specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder<TContext> UseFileContextDatabase<TContext, TStoreManager>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            string databaseName = "",
            string location = null,
            string password = null,
            [CanBeNull] FileContextDatabaseRoot databaseRoot = null,
            [CanBeNull] Action<FileContextDbContextOptionsBuilder> inMemoryOptionsAction = null)
            where TContext : DbContext
            where TStoreManager : IStoreManager
            => (DbContextOptionsBuilder<TContext>) UseFileContextDatabase<TStoreManager>(
                optionsBuilder, databaseName, location, password, databaseRoot, inMemoryOptionsAction);

        /// <summary>
        ///     Configures the context to connect to a named in-memory database.
        ///     The in-memory database is shared anywhere the same name is used, but only for a given
        ///     service provider.
        /// </summary>
        /// <typeparam name="TStoreManager"> The type of the store manager to be used. </typeparam>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="databaseName">
        ///     The name of the in-memory database. This allows the scope of the in-memory database to be controlled
        ///     independently of the context. The in-memory database is shared anywhere the same name is used.
        /// </param>
        /// <param name="location">An optional parameter to define the location where the files are stored</param>
        /// <param name="password">An optional parameter to define a password if needed by store manager, serializer or file manager</param>
        /// <param name="databaseRoot">
        ///     All in-memory databases will be rooted in this object, allowing the application
        ///     to control their lifetime. This is useful when sometimes the context instance
        ///     is created explicitly with <c>new</c> while at other times it is resolved using dependency injection.
        /// </param>
        /// <param name="inMemoryOptionsAction">An optional action to allow additional in-memory specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder UseFileContextDatabase<TStoreManager>(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            string databaseName = "",
            string location = null,
            string password = null,
            [CanBeNull] FileContextDatabaseRoot databaseRoot = null,
            [CanBeNull] Action<FileContextDbContextOptionsBuilder> inMemoryOptionsAction = null)
            where TStoreManager : IStoreManager
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));

            var extension = optionsBuilder.Options.FindExtension<FileContextOptionsExtension>()
                            ?? new FileContextOptionsExtension();

            extension = extension.WithCustomOptions(databaseName, location, password, typeof(TStoreManager));

            if (databaseRoot != null)
            {
                extension = extension.WithDatabaseRoot(databaseRoot);
            }

            ConfigureWarnings(optionsBuilder);

            ((IDbContextOptionsBuilderInfrastructure) optionsBuilder).AddOrUpdateExtension(extension);

            inMemoryOptionsAction?.Invoke(new FileContextDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }


        #endregion
        
        private static void ConfigureWarnings(DbContextOptionsBuilder optionsBuilder)
        {
            // Set warnings defaults
            var coreOptionsExtension
                = optionsBuilder.Options.FindExtension<CoreOptionsExtension>()
                  ?? new CoreOptionsExtension();

            coreOptionsExtension = coreOptionsExtension.WithWarningsConfiguration(
                coreOptionsExtension.WarningsConfiguration.TryWithExplicit(
                    FileContextEventId.TransactionIgnoredWarning, WarningBehavior.Throw));

            ((IDbContextOptionsBuilderInfrastructure) optionsBuilder).AddOrUpdateExtension(coreOptionsExtension);
        }
    }
}