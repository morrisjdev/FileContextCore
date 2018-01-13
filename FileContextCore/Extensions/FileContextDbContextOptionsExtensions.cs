// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using FileContextCore.Infrastructure.Internal;
using FileContextCore.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

// ReSharper disable once CheckNamespace
namespace FileContextCore.Extensions
{
	/// <summary>
	///     In-memory specific extension methods for <see cref="DbContextOptionsBuilder" />.
	/// </summary>
	public static class FileContextDbContextOptionsExtensions
	{
		//private const string LegacySharedName = "___Shared_Database___";
		
		/// <summary>
		///     Configures the context to use FileContext.
		///     The data are saved into files.
		/// </summary>
		/// <typeparam name="TContext"> The type of context being configured. </typeparam>
		/// <param name="optionsBuilder"> The builder being used to configure the context. </param>
		/// <param name="serializer">
		///     The name of the serializer. This allows the use of different serializers for different
		///     scenarios and preferences.
		/// </param>
		/// <param name="filemanager">The selection the of the file-manager to encrypt the files for example.</param>
		/// <returns> The options builder so that further configuration can be chained. </returns>
		public static DbContextOptionsBuilder<TContext> UseFileContext<TContext>(
			[NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
			string serializer = "json", string filemanager = "default")
			where TContext : DbContext
			=> (DbContextOptionsBuilder<TContext>)UseFileContext(
				(DbContextOptionsBuilder)optionsBuilder, serializer, filemanager);

		/// <summary>
		///     Configures the context to use FileContext.
		///     The data are saved into files.
		/// </summary>
		/// <param name="optionsBuilder"> The builder being used to configure the context. </param>
		/// <param name="serializer">
		///     The name of the serializer. This allows the use of different serializers for different
		///     scenarios and preferences.
		/// </param>
		/// <param name="filemanager">The selection the of the file-manager to encrypt the files for example.</param>
		/// <returns> The options builder so that further configuration can be chained. </returns>
		public static DbContextOptionsBuilder UseFileContext(
			[NotNull] this DbContextOptionsBuilder optionsBuilder, string serializer = "json", string filemanager = "default")
		{
			Check.NotNull(optionsBuilder, nameof(optionsBuilder));

			FileContextOptionsExtension extension = optionsBuilder.Options.FindExtension<FileContextOptionsExtension>()
				?? new FileContextOptionsExtension();

			extension = extension.WithSerializerAndFileManager(serializer, filemanager);

			((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

			return optionsBuilder;
		}
	}
}
