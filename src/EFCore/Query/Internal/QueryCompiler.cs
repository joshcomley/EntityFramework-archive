// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.Logging;
using Remotion.Linq;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Parsing.Structure.ExpressionTreeProcessors;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.Query.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class QueryCompiler : IQueryCompiler
    {
        private readonly IQueryFilterTypesCache _queryFilterTypesCache;
        private readonly IQueryFiltersCacheKeyGenerator _queryFiltersCacheKeyGenerator;
        private readonly IFinalQueryCacheKeyGenerator _finalQueryCacheKeyGenerator;
        [NotNull]
        private readonly IServiceProvider _serviceProvider;
        private readonly IQueryParserFactory _queryParserFactory;

        private static MethodInfo CompileQueryMethod { get; }
        = typeof(IDatabase).GetTypeInfo()
            .GetDeclaredMethod(nameof(IDatabase.CompileQuery));

        private static readonly IEvaluatableExpressionFilter _evaluatableExpressionFilter
            = new EvaluatableExpressionFilter();

        private readonly IQueryContextFactory _queryContextFactory;
        private readonly ICompiledQueryCache _compiledQueryCache;
        private readonly ICompiledQueryCacheKeyGenerator _compiledQueryCacheKeyGenerator;
        private readonly IDatabase _database;
        private readonly ISensitiveDataLogger _logger;
        private readonly INodeTypeProviderFactory _nodeTypeProviderFactory;
        private readonly Type _contextType;

        private INodeTypeProvider _nodeTypeProvider;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public QueryCompiler(
            [NotNull] IQueryContextFactory queryContextFactory,
            [NotNull] ICompiledQueryCache compiledQueryCache,
            [NotNull] ICompiledQueryCacheKeyGenerator compiledQueryCacheKeyGenerator,
            [NotNull] IQueryFiltersCacheKeyGenerator queryFiltersCacheKeyGenerator,
            [NotNull] IDatabase database,
            [NotNull] ISensitiveDataLogger<QueryCompiler> logger,
            [NotNull] INodeTypeProviderFactory nodeTypeProviderFactory,
            [NotNull] ICurrentDbContext currentContext,
            [NotNull] IQueryParserFactory queryParserFactory,
            [NotNull] IQueryFilterTypesCache queryFilterTypesCache,
            [NotNull] IFinalQueryCacheKeyGenerator finalQueryCacheKeyGenerator,
            [NotNull] IServiceProvider serviceProvider,
            [NotNull] IModel model)
        {
            Check.NotNull(queryContextFactory, nameof(queryContextFactory));
            Check.NotNull(compiledQueryCache, nameof(compiledQueryCache));
            Check.NotNull(compiledQueryCacheKeyGenerator, nameof(compiledQueryCacheKeyGenerator));
            Check.NotNull(queryFiltersCacheKeyGenerator, nameof(queryFiltersCacheKeyGenerator));
            Check.NotNull(database, nameof(database));
            Check.NotNull(logger, nameof(logger));
            Check.NotNull(currentContext, nameof(currentContext));
            Check.NotNull(queryParserFactory, nameof(queryParserFactory));
            Check.NotNull(queryFilterTypesCache, nameof(queryFilterTypesCache));
            Check.NotNull(finalQueryCacheKeyGenerator, nameof(finalQueryCacheKeyGenerator));
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            _queryContextFactory = queryContextFactory;
            _compiledQueryCache = compiledQueryCache;
            _compiledQueryCacheKeyGenerator = compiledQueryCacheKeyGenerator;
            _queryFiltersCacheKeyGenerator = queryFiltersCacheKeyGenerator;
            _database = database;
            _logger = logger;
            _queryParserFactory = queryParserFactory;
            _queryFilterTypesCache = queryFilterTypesCache;
            _finalQueryCacheKeyGenerator = finalQueryCacheKeyGenerator;
            _serviceProvider = serviceProvider;
            _nodeTypeProviderFactory = nodeTypeProviderFactory;
            _contextType = currentContext.Context.GetType();
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual IDatabase Database => _database;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual TResult Execute<TResult>(Expression query)
        {
            Check.NotNull(query, nameof(query));

            var queryContext = _queryContextFactory.Create();

            query = ExtractParameters(query, queryContext);

            var queryFilters = NewQueryFilters(queryContext);

            Func<Func<QueryContext, TResult>> compiler =
                () => CompileQueryCore<TResult>(queryFilters, query, NodeTypeProvider, _database, _logger, _contextType);
            var cache = CacheAndOrGetQuery(queryFilters, query, compiler, false);
            var compiledQuery =
                _compiledQueryCache.GetOrAddQuery(
                    cache.Item1,
                    () => cache.Item2 ?? compiler());

            return compiledQuery(queryContext);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual Func<QueryContext, TResult> CreateCompiledQuery<TResult>(Expression query)
        {
            Check.NotNull(query, nameof(query));

            var queryContext = _queryContextFactory.Create();

            query = ExtractParameters(query, queryContext, parameterize: false);

            return CompileQueryCore<TResult>(NewQueryFilters(queryContext), query, NodeTypeProvider, _database, _logger, _contextType);
        }

        private static Func<QueryContext, TResult> CompileQueryCore<TResult>(
            IQueryFilters queryFilters,
            Expression query, INodeTypeProvider nodeTypeProvider, IDatabase database, ILogger logger, Type contextType)
        {
            var queryModel = GetQueryModel(query, nodeTypeProvider);

            var resultItemType
                = (queryModel.GetOutputDataInfo()
                      as StreamedSequenceInfo)?.ResultItemType
                  ?? typeof(TResult);

            if (resultItemType == typeof(TResult))
            {
                var compiledQuery = database.CompileQuery<TResult>(queryModel, queryFilters);

                return qc =>
                    {
                        try
                        {
                            return compiledQuery(qc).First();
                        }
                        catch (Exception exception)
                        {
                            logger
                                .LogError(
                                    CoreEventId.DatabaseError,
                                    () => new DatabaseErrorLogState(contextType),
                                    exception,
                                    e => CoreStrings.LogExceptionDuringQueryIteration(Environment.NewLine, e));

                            throw;
                        }
                    };
            }

            try
            {
                return (Func<QueryContext, TResult>)CompileQueryMethod
                    .MakeGenericMethod(resultItemType)
                    .Invoke(database, new object[] { queryModel, queryFilters });
            }
            catch (TargetInvocationException e)
            {
                ExceptionDispatchInfo.Capture(e.InnerException).Throw();

                throw;
            }
        }

        private Tuple<object, TFunc> CacheAndOrGetQuery<TFunc>(
            IQueryFilters queryFilters,
                  Expression query, Func<TFunc> compiler, bool async)
        {
            var queryCacheKey = _compiledQueryCacheKeyGenerator.GenerateCacheKey(query, async);
            var filterTypesCached = _queryFilterTypesCache.IsCached(queryCacheKey);
            var compiled = default(TFunc);
            if (!filterTypesCached)
            {
                compiled = compiler();
                foreach (var typeFilters in queryFilters.All)
                {
                    _queryFilterTypesCache.AddQueryFilter(queryCacheKey, typeFilters.Type);
                }
            }
            var queryFilterList = new List<Expression>();
            foreach (var type in _queryFilterTypesCache.GetQueryFilters(queryCacheKey))
            {
                queryFilterList.AddRange(queryFilters.ResolveForType(type).ParameterizedExpressions);
            }
            var filterCacheKey = _queryFiltersCacheKeyGenerator.GenerateCacheKey(queryFilterList);
            var finalQueryCacheKey = _finalQueryCacheKeyGenerator.GenerateCacheKey(queryCacheKey, filterCacheKey);
            return new Tuple<object, TFunc>(finalQueryCacheKey, compiled);
        }

        private static QueryModel GetQueryModel(Expression query, INodeTypeProvider nodeTypeProvider)
        {
            var queryModel
                = CreateQueryParser(nodeTypeProvider)
                    .GetParsedQuery(query);
            return queryModel;
        }
        
        /// <summary>
         ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
         ///     directly from your code. This API may change or be removed in future releases.
         /// </summary>
        public virtual IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression query)
        {
            Check.NotNull(query, nameof(query));

            var queryContext = _queryContextFactory.Create();

            query = ExtractParameters(query, queryContext);

            var queryFilters = NewQueryFilters(queryContext);

            Func<Func<QueryContext, IAsyncEnumerable<TResult>>> compiler =
                () => CompileAsyncQueryCore<TResult>(queryFilters, query, NodeTypeProvider, _database);

            var cache = CacheAndOrGetQuery(queryFilters, query, compiler, true);
            var compiledQuery =
                _compiledQueryCache.GetOrAddQuery(
                    cache.Item1,
                    () => cache.Item2 ?? compiler());
            return compiledQuery(queryContext);
        }

        private IQueryFilters NewQueryFilters(QueryContext queryContext)
        {
            var queryFilters = _serviceProvider.GetService<IQueryFilters>();
            queryFilters.SetParameterize(expression => ExtractParameters(expression, queryContext));
            return queryFilters;
        }
        
        /// <summary>
                 ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
                 ///     directly from your code. This API may change or be removed in future releases.
                 /// </summary>
        public virtual Func<QueryContext, IAsyncEnumerable<TResult>> CreateCompiledAsyncEnumerableQuery<TResult>(Expression query)
        {
            Check.NotNull(query, nameof(query));

            var queryContext = _queryContextFactory.Create();

            query = ExtractParameters(query, queryContext, parameterize: false);

            return CompileAsyncQueryCore<TResult>(NewQueryFilters(queryContext), query, NodeTypeProvider, _database);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual Func<QueryContext, Task<TResult>> CreateCompiledAsyncTaskQuery<TResult>(Expression query)
        {
            Check.NotNull(query, nameof(query));

            var queryContext = _queryContextFactory.Create();

            query = ExtractParameters(query, queryContext, parameterize: false);

            var compiledQuery = CompileAsyncQueryCore<TResult>(NewQueryFilters(queryContext), query, NodeTypeProvider, _database);

            return CreateCompiledSingletonAsyncQuery(compiledQuery, _logger, _contextType);
        }

        private static Func<QueryContext, Task<TResult>> CreateCompiledSingletonAsyncQuery<TResult>(
                Func<QueryContext, IAsyncEnumerable<TResult>> compiledQuery, ILogger logger, Type contextType)
            => qc => ExecuteSingletonAsyncQuery(qc, compiledQuery, logger, contextType);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual Task<TResult> ExecuteAsync<TResult>(Expression query, CancellationToken cancellationToken)
        {
            Check.NotNull(query, nameof(query));

            var queryContext = _queryContextFactory.Create();

            queryContext.CancellationToken = cancellationToken;

            query = ExtractParameters(query, queryContext);

            var compiledQuery = CompileAsyncQuery<TResult>(query, queryContext);

            return ExecuteSingletonAsyncQuery(queryContext, compiledQuery, _logger, _contextType);
        }

        private static async Task<TResult> ExecuteSingletonAsyncQuery<TResult>(
            QueryContext queryContext,
            Func<QueryContext, IAsyncEnumerable<TResult>> compiledQuery,
            ILogger logger,
            Type contextType)
        {
            try
            {
                var asyncEnumerable = compiledQuery(queryContext);

                using (var asyncEnumerator = asyncEnumerable.GetEnumerator())
                {
                    await asyncEnumerator.MoveNext(queryContext.CancellationToken);

                    return asyncEnumerator.Current;
                }
            }
            catch (Exception exception)
            {
                logger
                    .LogError(
                        CoreEventId.DatabaseError,
                        () => new DatabaseErrorLogState(contextType),
                        exception,
                        e => CoreStrings.LogExceptionDuringQueryIteration(Environment.NewLine, e));

                throw;
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual Func<QueryContext, IAsyncEnumerable<TResult>> CompileAsyncQuery<TResult>([NotNull] Expression query,
            [NotNull]QueryContext queryContext)
        {
            Check.NotNull(query, nameof(query));

            var queryFilters = NewQueryFilters(queryContext);

            Func<Func<QueryContext, IAsyncEnumerable<TResult>>> compiler =
                () => _database.CompileAsyncQuery<TResult>(GetQueryModel(query, NodeTypeProvider), queryFilters);
            var cache = CacheAndOrGetQuery(queryFilters, query, compiler, true);
            return _compiledQueryCache.GetOrAddAsyncQuery(
                cache.Item1,
                () => cache.Item2 ?? compiler());
        }

        private static Func<QueryContext, IAsyncEnumerable<TResult>> CompileAsyncQueryCore<TResult>(
            IQueryFilters queryFilters,
            Expression query, INodeTypeProvider nodeTypeProvider, IDatabase database)
        {
            var queryModel
                = CreateQueryParser(nodeTypeProvider)
                    .GetParsedQuery(query);

            return database.CompileAsyncQuery<TResult>(queryModel, queryFilters);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual Expression ExtractParameters(
            [NotNull] Expression query,
            [NotNull] QueryContext queryContext,
            bool parameterize = true)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(queryContext, nameof(queryContext));

            return ParameterExtractingExpressionVisitor
                .ExtractParameters(
                    query,
                    queryContext,
                    _evaluatableExpressionFilter,
                    _logger,
                    parameterize);
        }

        private static QueryParser CreateQueryParser(INodeTypeProvider nodeTypeProvider)
            => new QueryParser(
                new ExpressionTreeParser(
                    nodeTypeProvider,
                    new CompoundExpressionTreeProcessor(new IExpressionTreeProcessor[]
                    {
                        new PartialEvaluatingExpressionTreeProcessor(_evaluatableExpressionFilter),
                        new TransformingExpressionTreeProcessor(ExpressionTransformerRegistry.CreateDefault())
                    })));

        private INodeTypeProvider NodeTypeProvider
            => _nodeTypeProvider
               ?? (_nodeTypeProvider = _nodeTypeProviderFactory.Create());
    }
}
