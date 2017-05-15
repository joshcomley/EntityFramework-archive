using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Microsoft.EntityFrameworkCore.Query
{
    /// <summary>
    ///     <para>
    ///         Creates keys that uniquely identifies a query filter. This is used to store and lookup
    ///         compiled versions of a query filter in a cache.
    ///     </para>
    ///     <para>
    ///         This type is typically used by database providers (and other extensions). It is generally
    ///         not used in application code.
    ///     </para>
    /// </summary>
    public class QueryFiltersCacheKeyGenerator : IQueryFiltersCacheKeyGenerator
    {
        /// <summary>
        ///     <para>
        ///         A key that uniquely identifies a query filter. This is used to store and lookup
        ///         compiled versions of a query filter in a cache. 
        ///     </para>
        ///     <para>
        ///         This type is typically used by database providers (and other extensions). It is generally
        ///         not used in application code.
        ///     </para>
        /// </summary>
        protected struct CompiledQueryFilterCacheKey
        {
            private static readonly ExpressionEqualityComparer _expressionEqualityComparer
                = new ExpressionEqualityComparer();

            private readonly ICollection<Expression> _queries;

            /// <summary>
            ///     Initializes a new instance of the <see cref="CompiledQueryFilterCacheKey"/> class.
            /// </summary>
            /// <param name="queries"> The query expressions. </param>
            public CompiledQueryFilterCacheKey(ICollection<Expression> queries)
            {
                _queries = queries;
            }

            /// <summary>
            ///     Determines if this key is equivalent to a given object (i.e. if they are keys for the same query and query filter).
            /// </summary>
            /// <param name="obj">
            ///     The object to compare this key to.
            /// </param>
            /// <returns>
            ///     True if the object is a <see cref="CompiledQueryFilterCacheKey"/> and is for the same query, otherwise false.
            /// </returns>
            public override bool Equals(object obj)
            {
                var other = (CompiledQueryFilterCacheKey)obj;
                if (other._queries.Count != _queries.Count)
                {
                    return false;
                }
                return !_queries.Where((t, i) => !_expressionEqualityComparer.Equals(t, other._queries.Skip(i).First())).Any();
            }

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
                    var hashCode = -1;
                    foreach (var expression in _queries)
                    {
                        if (hashCode == -1)
                        {
                            hashCode = _expressionEqualityComparer.GetHashCode(expression);
                        }
                        else
                        {
                            hashCode = (hashCode * 397) ^ _expressionEqualityComparer.GetHashCode(expression);
                        }
                    }
                    return hashCode;
                }
            }
        }

        /// <summary>
        ///     Generates the cache key for the given query.
        /// </summary>
        /// <param name="queries">The queries to get the cache key for.</param>
        /// <returns> The cache key. </returns>
        public virtual object GenerateCacheKey(ICollection<Expression> queries)
        {
            return new CompiledQueryFilterCacheKey(queries);
        }
    }
}