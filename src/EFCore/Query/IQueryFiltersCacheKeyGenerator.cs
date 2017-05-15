using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Query
{
    /// <summary>
    ///     Generates cache keys for entity filters
    /// </summary>
    public interface IQueryFiltersCacheKeyGenerator
    {
        /// <summary>
        ///     Generates the cache key
        /// </summary>
        /// <param name="queries">The entity filter expressions to generate the key for.</param>
        /// <returns></returns>
        object GenerateCacheKey([NotNull] ICollection<Expression> queries);
    }
}