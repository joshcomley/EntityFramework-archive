using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Query
{
    /// <summary>
    ///     A collection of entity filters
    /// </summary>
    public interface IQueryFilters
    {
        /// <summary>
        ///     Finds all filters for a given entity type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        TypeFilters ResolveForType([NotNull]Type type);

        /// <summary>
        ///     A function to parameterize entity filters
        /// </summary>
        Func<Expression, Expression> Parameterize { get; }

        /// <summary>
        ///     Set the parameterize function
        /// </summary>
        /// <param name="expression"></param>
        void SetParameterize([NotNull]Func<Expression, Expression> expression);

        /// <summary>
        ///     Returns all filters for all entity types
        /// </summary>
        ReadOnlyCollection<TypeFilters> All { get; }
    }
}