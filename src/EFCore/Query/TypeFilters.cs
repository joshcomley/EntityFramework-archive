using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Query
{
    /// <summary>
    ///     A collection of filters for a given entity type
    /// </summary>
    public class TypeFilters
    {
        private readonly IQueryFilters _queryFilters;
        private readonly IModel _model;

        /// <summary>
        /// The type the filters belong to
        /// </summary>
        public virtual Type Type { get; }

        /// <summary>
        /// Parameterised versions of the filter expressions
        /// </summary>
        public virtual ICollection<Expression> ParameterizedExpressions { get; } = new List<Expression>();

        /// <summary>
        /// Creates a new instance of the <see cref="TypeFilters"/> class
        /// </summary>
        /// <param name="type"></param>
        /// <param name="queryFilters"></param>
        /// <param name="model"></param>
        public TypeFilters([NotNull]Type type, [NotNull]IQueryFilters queryFilters, [NotNull]IModel model)
        {
            _queryFilters = queryFilters;
            _model = model;
            Type = type;
        }

        /// <summary>
        /// Populates the parameterized expressions
        /// </summary>
        public virtual void Populate()
        {
            var filters = _model.FindEntityType(Type)?.Filters;
            if (filters == null)
            {
                return;
            }
            foreach (var filter in filters)
            {
                var resolve = new Func<Expression>(() => filter(new EntityFilterContext(_model.FiltersServiceProvider)));
                var predicate = resolve();
                if (predicate == null)
                {
                    continue;
                }
                predicate = _queryFilters.Parameterize(predicate);
                ParameterizedExpressions.Add(predicate);
            }
        }
    }
}