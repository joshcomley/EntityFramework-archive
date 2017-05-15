using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Query
{
    /// <summary>
    ///     An entity filter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQueryFilter<T>
    {
        /// <summary>
        /// The filter expression
        /// </summary>
        /// <returns></returns>
        Expression<Func<T, bool>> Filter([NotNull]EntityFilterContext context);
    }
}