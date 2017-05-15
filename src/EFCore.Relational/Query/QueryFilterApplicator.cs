using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure.NodeTypeProviders;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class QueryFilterApplicator
    {
        private readonly IQueryParserFactory _queryParserFactory;
        private readonly IQueryFilters _queryFilters;
        private readonly ISqlTranslatingExpressionVisitorFactory _sqlTranslatingExpressionVisitorFactory;
        private readonly RelationalQueryModelVisitor _relationalQueryModelVisitor;
        public QueryFilterApplicator(
            [NotNull] IQueryParserFactory queryParserFactory, 
            [NotNull] IQueryFilters queryFilters, 
            [NotNull] ISqlTranslatingExpressionVisitorFactory sqlTranslatingExpressionVisitorFactory, 
            [NotNull] RelationalQueryModelVisitor relationalQueryModelVisitor)
        {
            if (queryParserFactory == null)
            {
                throw new ArgumentNullException(nameof(queryParserFactory));
            }
            if (queryFilters == null)
            {
                throw new ArgumentNullException(nameof(queryFilters));
            }
            if (sqlTranslatingExpressionVisitorFactory == null)
            {
                throw new ArgumentNullException(nameof(sqlTranslatingExpressionVisitorFactory));
            }
            if (relationalQueryModelVisitor == null)
            {
                throw new ArgumentNullException(nameof(relationalQueryModelVisitor));
            }
            _queryParserFactory = queryParserFactory;
            _queryFilters = queryFilters;
            _sqlTranslatingExpressionVisitorFactory = sqlTranslatingExpressionVisitorFactory;
            _relationalQueryModelVisitor = relationalQueryModelVisitor;
        }

        public virtual Expression ApplyFilterPredicates(
            [NotNull]Type itemType, 
            [CanBeNull]Expression predicate, 
            [CanBeNull]SelectExpression selectExpression = null)
        {
            var filterPredicates = GetFilterPredicates(itemType, selectExpression);
            if (filterPredicates == null)
            {
                return predicate;
            }
            if (predicate == null)
            {
                return filterPredicates;
            }
            predicate = Expression.AndAlso(
                predicate,
                filterPredicates
            );
            return predicate;
        }

        public virtual void ApplyFilterPredicates([NotNull]Type itemType, [NotNull]SelectExpression selectExpression, [CanBeNull]SqlTranslatingExpressionVisitor sqlTranslatingExpressionVisitor = null)
        {
            var filterPredicates = GetFilterPredicates(itemType);
            if (filterPredicates == null)
            {
                return;
            }
            if (selectExpression.Predicate != null)
            {
                filterPredicates = Expression.AndAlso(
                    selectExpression.Predicate,
                    filterPredicates
                );
            }
            selectExpression.Predicate = filterPredicates;
        }

        private Expression GetFilterPredicates(Type entityType, SelectExpression selectExpression = null)
        {
            var filters = GetFilters(entityType);
            var sql = _sqlTranslatingExpressionVisitorFactory.Create(_relationalQueryModelVisitor);
            Expression filterPredicates = null;
            foreach (var filter in filters)
            {
                _relationalQueryModelVisitor.PredicateFilterSelectExpression = selectExpression;
                var filterPredicate = sql.Visit(filter.Predicate);
                _relationalQueryModelVisitor.PredicateFilterSelectExpression = null;
                if (filterPredicate == null)
                {
                    continue;
                }
                if (filterPredicates == null)
                {
                    filterPredicates = filterPredicate;
                }
                else
                {
                    filterPredicates = Expression.AndAlso(
                        filterPredicates,
                        filterPredicate
                    );
                }
            }
            return filterPredicates;
        }

        private static readonly MethodInfo ResolveFilterExpressionsMethod = typeof(QueryFilterApplicator).GetTypeInfo().GetDeclaredMethod(nameof(ResolveFilterExpressions));

        public virtual ICollection<WhereClause> GetFilters([NotNull]Type entityType)
        {
            return (ICollection<WhereClause>)
                ResolveFilterExpressionsMethod
                    .MakeGenericMethod(entityType)
                    .Invoke(this, null);
        }

        //private readonly Dictionary<Type, ICollection<WhereClause>> _filtersResolved = new Dictionary<Type, ICollection<WhereClause>>();
        private ICollection<WhereClause> ResolveFilterExpressions<T>()
        {
            return _queryFilters
                .ResolveForType(typeof(T))
                .ParameterizedExpressions
                .Select(predicate => ConvertEntityTypeFilterToWhereClause((Expression<Func<T, bool>>)predicate))
                .ToList();
        }

        private WhereClause ConvertEntityTypeFilterToWhereClause<T>(Expression<Func<T, bool>> predicate)
        {
            var expression = new T[] { }.AsQueryable().Where(predicate).Expression;
            var queryModel
                = _queryParserFactory.CreateQueryParser()
                    .GetParsedQuery(expression);
            var whereClause = queryModel.BodyClauses.Single() as WhereClause;
            return whereClause;
        }
    }
}