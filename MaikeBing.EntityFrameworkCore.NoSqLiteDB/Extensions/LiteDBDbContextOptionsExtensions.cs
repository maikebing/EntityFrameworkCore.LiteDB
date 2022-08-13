// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MaikeBing.EntityFrameworkCore.NoSqLiteDB.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore;
using MaikeBing.EntityFrameworkCore.Infrastructure;
using MaikeBing.EntityFrameworkCore.Storage;
using MaikeBing.EntityFrameworkCore.NoSqLiteDB.Extensions.Internal;

// ReSharper disable once CheckNamespace
namespace MaikeBing.EntityFrameworkCore
{
    /// <summary>
    ///     In-memory specific extension methods for <see cref="DbContextOptionsBuilder" />.
    /// </summary>
    public static class LiteDBDbContextOptionsExtensions
    {
        private const string LegacySharedName = "___Shared_Database___";

        /// <summary>
        ///     Configures the context to connect to an in-memory database.
        ///     The in-memory database is shared anywhere the same name is used, but only for a given
        ///     service provider. To use the same in-memory database across service providers, call
        ///     <see
        ///         cref="UseLiteDB{TContext}(DbContextOptionsBuilder{TContext},string,LiteDBDatabaseRoot,Action{LiteDBDbContextOptionsBuilder})" />
        ///     passing a shared <see cref="LiteDBDatabaseRoot" /> on which to root the database.
        /// </summary>
        /// <typeparam name="TContext"> The type of context being configured. </typeparam>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="databaseName">
        ///     The name of the in-memory database. This allows the scope of the in-memory database to be controlled
        ///     independently of the context. The in-memory database is shared anywhere the same name is used.
        /// </param>
        /// <param name="inMemoryOptionsAction">An optional action to allow additional in-memory specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder<TContext> UseLiteDB<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            [NotNull] string databaseName,
            [CanBeNull] Action<LiteDBDbContextOptionsBuilder> inMemoryOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UseLiteDB(
                (DbContextOptionsBuilder)optionsBuilder, databaseName, inMemoryOptionsAction);

        /// <summary>
        ///     Configures the context to connect to a named in-memory database.
        ///     The in-memory database is shared anywhere the same name is used, but only for a given
        ///     service provider. To use the same in-memory database across service providers, call
        ///     <see cref="UseLiteDB(DbContextOptionsBuilder,string,LiteDBDatabaseRoot,Action{LiteDBDbContextOptionsBuilder})" />
        ///     passing a shared <see cref="LiteDBDatabaseRoot" /> on which to root the database.
        /// </summary>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="databaseName">
        ///     The name of the in-memory database. This allows the scope of the in-memory database to be controlled
        ///     independently of the context. The in-memory database is shared anywhere the same name is used.
        /// </param>
        /// <param name="inMemoryOptionsAction">An optional action to allow additional in-memory specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder UseLiteDB(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [NotNull] string databaseName,
            [CanBeNull] Action<LiteDBDbContextOptionsBuilder> inMemoryOptionsAction = null)
            => UseLiteDB(optionsBuilder, databaseName, null, inMemoryOptionsAction);

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
        /// <param name="databaseRoot">
        ///     All in-memory databases will be rooted in this object, allowing the application
        ///     to control their lifetime. This is useful when sometimes the context instance
        ///     is created explicitly with <c>new</c> while at other times it is resolved using dependency injection.
        /// </param>
        /// <param name="inMemoryOptionsAction">An optional action to allow additional in-memory specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder<TContext> UseLiteDB<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            [NotNull] string databaseName,
            [CanBeNull] LiteDBDatabaseRoot databaseRoot,
            [CanBeNull] Action<LiteDBDbContextOptionsBuilder> inMemoryOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UseLiteDB(
                (DbContextOptionsBuilder)optionsBuilder, databaseName, databaseRoot, inMemoryOptionsAction);

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
        /// <param name="databaseRoot">
        ///     All in-memory databases will be rooted in this object, allowing the application
        ///     to control their lifetime. This is useful when sometimes the context instance
        ///     is created explicitly with <c>new</c> while at other times it is resolved using dependency injection.
        /// </param>
        /// <param name="inMemoryOptionsAction">An optional action to allow additional in-memory specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder UseLiteDB(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [NotNull] string databaseName,
            [CanBeNull] LiteDBDatabaseRoot databaseRoot,
            [CanBeNull] Action<LiteDBDbContextOptionsBuilder> inMemoryOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));
            Check.NotEmpty(databaseName, nameof(databaseName));
           
            var extension = optionsBuilder.Options.FindExtension<LiteDBOptionsExtension>()??new LiteDBOptionsExtension(new LiteDBDbContextOptionsExtension());

  extension = extension.WithStoreName(databaseName);

            if (databaseRoot != null)
            {
                extension = extension.WithDatabaseRoot(databaseRoot);
            }

            ConfigureWarnings(optionsBuilder);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            inMemoryOptionsAction?.Invoke(new LiteDBDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        /// <summary>
        ///     Configures the context to connect to the legacy shared in-memory database.
        ///     This method is obsolete. Use
        ///     <see cref="UseLiteDB{TContext}(DbContextOptionsBuilder{TContext},string,Action{LiteDBDbContextOptionsBuilder})" /> instead.
        /// </summary>
        /// <typeparam name="TContext"> The type of context being configured. </typeparam>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="inMemoryOptionsAction">An optional action to allow additional in-memory specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        [Obsolete("Use UseLiteDB(string, LiteDBDatabaseRoot) instead.")]
        public static DbContextOptionsBuilder<TContext> UseLiteDB<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            [CanBeNull] Action<LiteDBDbContextOptionsBuilder> inMemoryOptionsAction = null)
            where TContext : DbContext
            => optionsBuilder.UseLiteDB(LegacySharedName, null, inMemoryOptionsAction);

        /// <summary>
        ///     Configures the context to connect to the legacy shared in-memory database.
        ///     This method is obsolete. Use <see cref="UseLiteDB(DbContextOptionsBuilder,string,Action{LiteDBDbContextOptionsBuilder})" />
        ///     instead.
        /// </summary>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="inMemoryOptionsAction">An optional action to allow additional in-memory specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        [Obsolete("Use UseLiteDB(string, LiteDBDatabaseRoot) instead.")]
        public static DbContextOptionsBuilder UseLiteDB(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [CanBeNull] Action<LiteDBDbContextOptionsBuilder> inMemoryOptionsAction = null)
            => optionsBuilder.UseLiteDB(LegacySharedName, null, inMemoryOptionsAction);

        private static void ConfigureWarnings(DbContextOptionsBuilder optionsBuilder)
        {
            // Set warnings defaults
            var coreOptionsExtension
                = optionsBuilder.Options.FindExtension<CoreOptionsExtension>()
                  ?? new CoreOptionsExtension();

            coreOptionsExtension = coreOptionsExtension.WithWarningsConfiguration(
                coreOptionsExtension.WarningsConfiguration.TryWithExplicit(
                    LiteDBEventId.TransactionIgnoredWarning, WarningBehavior.Throw));

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(coreOptionsExtension);
        }
    }
}
