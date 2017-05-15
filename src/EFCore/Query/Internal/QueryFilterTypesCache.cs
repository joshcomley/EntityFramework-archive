using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Caching.Memory;

namespace Microsoft.EntityFrameworkCore.Query.Internal
{
    /// <summary>
    ///     A cache of the types that so far have had their query filters cached
    /// </summary>
    public class QueryFilterTypesCache : IQueryFilterTypesCache
    {
        private static readonly object _compiledQueryLockObject = new object();

        private readonly IMemoryCache _memoryCache;

        /// <summary>
        ///     Creates a new instance of the <see cref="QueryFilterTypesCache"/> class.
        /// </summary>
        public QueryFilterTypesCache([NotNull] IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Determines if a particular key is cached
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public virtual bool IsCached(object cacheKey)
        {
            lock (_compiledQueryLockObject)
            {
                object filters;
                if (!_memoryCache.TryGetValue(cacheKey, out filters))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///     Adds an entity filter with a given key
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="type"></param>
        public virtual void AddQueryFilter(object cacheKey, Type type)
        {
            var filters = GetQueryFilters(cacheKey);
            lock (_compiledQueryLockObject)
            {
                if (!filters.Contains(type))
                {
                    filters.Add(type);
                }
            }
        }

        /// <summary>
        ///     Retrieves the entity filters cached by a given key
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public virtual ICollection<Type> GetQueryFilters(object cacheKey)
        {
            ICollection<Type> filters;
            lock (_compiledQueryLockObject)
            {
                if (!_memoryCache.TryGetValue(cacheKey, out filters))
                {
                    filters = new List<Type>();
                    _memoryCache.Set(cacheKey, filters);
                }
            }
            return filters;
        }
    }
}