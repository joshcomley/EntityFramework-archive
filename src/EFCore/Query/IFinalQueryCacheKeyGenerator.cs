using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Query
{
    /// <summary>
    ///     Generates cache keys combining both querys and entity filters
    /// </summary>
    public interface IFinalQueryCacheKeyGenerator
    {
        /// <summary>
        /// Generates the cache key
        /// </summary>
        /// <param name="queryCacheKey">The query cache key.</param>
        /// <param name="filterCacheKey">The filter cache key.</param>
        /// <returns></returns>
        object GenerateCacheKey([NotNull]object queryCacheKey, [NotNull]object filterCacheKey);
    }
}