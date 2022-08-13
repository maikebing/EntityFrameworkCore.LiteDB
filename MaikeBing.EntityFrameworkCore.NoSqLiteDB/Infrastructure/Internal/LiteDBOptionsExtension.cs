// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;
using MaikeBing.EntityFrameworkCore.Storage;
using MaikeBing.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace MaikeBing.EntityFrameworkCore.NoSqLiteDB.Infrastructure.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class LiteDBOptionsExtension : DbContextOptionsExtensionInfo, IDbContextOptionsExtension
    {
        private string _storeName;
        private LiteDBDatabaseRoot _databaseRoot;
        private string _logFragment;
        private IDbContextOptionsExtension _extension;

        public LiteDBOptionsExtension(IDbContextOptionsExtension extension) : base(extension)
        {
            _extension = extension;
        }

     



        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual LiteDBOptionsExtension Clone() => new LiteDBOptionsExtension(_extension);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual string StoreName => _storeName;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual LiteDBOptionsExtension WithStoreName([NotNull] string storeName)
        {
            var clone = Clone();

            clone._storeName = storeName;

            return clone;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual LiteDBDatabaseRoot DatabaseRoot => _databaseRoot;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual LiteDBOptionsExtension WithDatabaseRoot([NotNull] LiteDBDatabaseRoot databaseRoot)
        {
            var clone = Clone();

            clone._databaseRoot = databaseRoot;

            return clone;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public   void ApplyServices(IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));

            services.AddEntityFrameworkLiteDBDatabase();

        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override int  GetServiceProviderHashCode() => _databaseRoot?.GetHashCode() ?? 0;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {
            debugInfo["LiteDBDatabase:DatabaseRoot"] = (_databaseRoot?.GetHashCode() ?? 0L).ToString(CultureInfo.InvariantCulture);
        }

  
        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
        {
            return false;
        }

     

        public void Validate(IDbContextOptions options)
        {
            
        }

     






        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override string LogFragment
        {
            get
            {
                if (_logFragment == null)
                {
                    var builder = new StringBuilder();

                    builder.Append("StoreName=").Append(_storeName).Append(' ');

                    _logFragment = builder.ToString();
                }

                return _logFragment;
            }
        }

        public override bool IsDatabaseProvider => true;

        public DbContextOptionsExtensionInfo Info => _extension.Info;
    }
}
