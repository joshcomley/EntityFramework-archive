using System;
using System.Linq;

namespace Microsoft.EntityFrameworkCore.Query
{
    /// <summary>
    /// 
    /// </summary>
    public static class IQueryFiltersExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryFilters"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasForType(this IQueryFilters queryFilters, Type type) => queryFilters.ResolveForType(type).ParameterizedExpressions.Any();
    }
}