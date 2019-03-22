// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using JetBrains.Annotations;
using MaikeBing.EntityFrameworkCore.NoSqLiteDB.Query.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Clauses;

namespace MaikeBing.EntityFrameworkCore.NoSqLiteDB.Query.ExpressionVisitors.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class LiteDBEntityQueryableExpressionVisitorFactory : IEntityQueryableExpressionVisitorFactory
    {
        private readonly IModel _model;
        private readonly ILiteDBMaterializerFactory _materializerFactory;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public LiteDBEntityQueryableExpressionVisitorFactory(
            [NotNull] IModel model,
            [NotNull] ILiteDBMaterializerFactory materializerFactory)
        {
            Check.NotNull(model, nameof(model));
            Check.NotNull(materializerFactory, nameof(materializerFactory));

            _model = model;
            _materializerFactory = materializerFactory;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual ExpressionVisitor Create(
            EntityQueryModelVisitor queryModelVisitor, IQuerySource querySource)
            => new LiteDBEntityQueryableExpressionVisitor(
                _model,
                _materializerFactory,
                Check.NotNull(queryModelVisitor, nameof(queryModelVisitor)),
                querySource);
    }
}
