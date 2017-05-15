namespace Microsoft.EntityFrameworkCore.Query
{
    /// <summary>
    /// The final cache key generator combining the query and the filters
    /// </summary>
    public class FinalQueryCacheKeyGenerator : IFinalQueryCacheKeyGenerator
    {
        /// <summary>
        /// The final cache key combining the query and the filters
        /// </summary>
        protected struct FinalQueryFilterCacheKey
        {
            private readonly object _queryCacheKey;
            private readonly object _filtersCacheKey;

            /// <summary>
            ///     Initializes a new instance of the <see cref="FinalQueryFilterCacheKey"/> class.
            /// </summary>
            /// <param name="queryCacheKey">The query cache key.</param>
            /// <param name="filtersCacheKey">The filters cache key.</param>
            public FinalQueryFilterCacheKey(
                object queryCacheKey,
                object filtersCacheKey)
            {
                _queryCacheKey = queryCacheKey;
                _filtersCacheKey = filtersCacheKey;
            }

            /// <summary>
            ///     Determines if this key is equivalent to a given object (i.e. if they are keys for the same query and query filter).
            /// </summary>
            /// <param name="obj">
            ///     The object to compare this key to.
            /// </param>
            /// <returns>
            ///     True if the object is a <see cref="FinalQueryFilterCacheKey"/> and is for the same query, otherwise false.
            /// </returns>
            public override bool Equals(object obj)
                => !ReferenceEquals(null, obj)
                   && obj is FinalQueryFilterCacheKey
                   && Equals((FinalQueryFilterCacheKey)obj);

            private bool Equals(FinalQueryFilterCacheKey q) =>
                q._filtersCacheKey.GetHashCode() == _filtersCacheKey.GetHashCode() &&
                q._queryCacheKey.GetHashCode() == _queryCacheKey.GetHashCode();

            /// <summary>
            ///     Gets the hash code for the key.
            /// </summary>
            /// <returns>
            ///     The hash code for the key.
            /// </returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = _queryCacheKey.GetHashCode();
                    hashCode = (hashCode * 397) ^ _filtersCacheKey.GetHashCode();
                    return hashCode;
                }
            }
        }

        /// <summary>
        ///     Generates the cache key for the given query.
        /// </summary>
        /// <param name="queryCacheKey">The query cache key.</param>
        /// <param name="filterCacheKey">The filter cache key.</param>
        /// <returns> The cache key. </returns>
        public virtual object GenerateCacheKey(object queryCacheKey, object filterCacheKey)
        {
            return new FinalQueryFilterCacheKey(queryCacheKey, filterCacheKey);
        }
    }
}