using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MaikeBing.EntityFrameworkCore.NoSqLiteDB.Query.ExpressionVisitors.Internal
{
    public class LiteDBQueryableMethodTranslatingExpressionVisitor : RelationalQueryableMethodTranslatingExpressionVisitor
    {
        public LiteDBQueryableMethodTranslatingExpressionVisitor(
            QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
            RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies,
            QueryCompilationContext queryCompilationContext)
            : base(dependencies, relationalDependencies, queryCompilationContext)
        {
        }

        protected LiteDBQueryableMethodTranslatingExpressionVisitor(
            LiteDBQueryableMethodTranslatingExpressionVisitor parentVisitor)
            : base(parentVisitor)
        {
        }

        protected override QueryableMethodTranslatingExpressionVisitor CreateSubqueryVisitor()
            => new LiteDBQueryableMethodTranslatingExpressionVisitor(this);

        protected override ShapedQueryExpression TranslateOrderBy(
            ShapedQueryExpression source, LambdaExpression keySelector, bool ascending)
        {
            var translation = base.TranslateOrderBy(source, keySelector, ascending);
            if (translation == null)
            {
                return null;
            }

            var orderingExpression = ((SelectExpression)translation.QueryExpression).Orderings.Last();
            var orderingExpressionType = GetProviderType(orderingExpression.Expression);
            if (orderingExpressionType == typeof(DateTimeOffset)
                || orderingExpressionType == typeof(decimal)
                || orderingExpressionType == typeof(TimeSpan)
                || orderingExpressionType == typeof(ulong))
            {
                throw new NotSupportedException();
            }

            return translation;
        }

        protected override ShapedQueryExpression TranslateThenBy(ShapedQueryExpression source, LambdaExpression keySelector, bool ascending)
        {
            var translation = base.TranslateThenBy(source, keySelector, ascending);
            if (translation == null)
            {
                return null;
            }

            var orderingExpression = ((SelectExpression)translation.QueryExpression).Orderings.Last();
            var orderingExpressionType = GetProviderType(orderingExpression.Expression);
            if (orderingExpressionType == typeof(DateTimeOffset)
                || orderingExpressionType == typeof(decimal)
                || orderingExpressionType == typeof(TimeSpan)
                || orderingExpressionType == typeof(ulong))
            {
                throw new NotSupportedException(orderingExpressionType.ShortDisplayName());
            }

            return translation;
        }

        private static Type GetProviderType(SqlExpression expression)
            => (expression.TypeMapping?.Converter?.ProviderClrType
                ?? expression.TypeMapping?.ClrType
                ?? expression.Type).UnwrapNullableType();
    }
}
