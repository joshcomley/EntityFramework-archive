using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Query
{
    /// <summary>
    ///     The cache for entity filters
    /// </summary>
    public interface IQueryFilterTypesCache
    {
        /// <summary>
        ///     Determines if a particular query filter is cached using a key
        /// </summary>
        /// <param name="cacheKey">The key to lookup.</param>
        /// <returns></returns>
        bool IsCached(
            [NotNull] object cacheKey);

        /// <summary>
        ///     Adds a query filter to the cache with a given key
        /// </summary>
        /// <param name="cacheKey">The key to save with.</param>
        /// <param name="type">The entity type to cache the filter against.</param>
        void AddQueryFilter(
            [NotNull] object cacheKey,
            [NotNull] Type type);

        /// <summary>
        ///     Returns all entity types that have filters cached
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        ICollection<Type> GetQueryFilters(
            [NotNull] object cacheKey);
    }
}