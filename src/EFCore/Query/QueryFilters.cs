using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Query
{
    /// <summary>
    /// A collection of entity filters
    /// </summary>
    public class QueryFilters : IQueryFilters
    {
        private readonly IModel _model;

        /// <summary>
        /// Creates a new instance of the <see cref="QueryFilters"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public QueryFilters([NotNull]IModel model)
        {
            _model = model;
        }

        private ReadOnlyCollection<TypeFilters> _all = new ReadOnlyCollection<TypeFilters>(new List<TypeFilters>());
        private Func<Expression, Expression> _parameterize;

        private Dictionary<Type, TypeFilters> TypeFilters { get; } = new Dictionary<Type, TypeFilters>();

        /// <summary>
        /// Resolves the registered filters for a given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual TypeFilters ResolveForType(Type type)
        {
            if (!TypeFilters.ContainsKey(type))
            {
                var typeFilters = new TypeFilters(type, this, _model);
                TypeFilters.Add(type, typeFilters);
                typeFilters.Populate();
                _all = TypeFilters.Values.ToList().AsReadOnly();
            }
            return TypeFilters[type];
        }

        /// <summary>
        ///     Set the parameterize function
        /// </summary>
        /// <param name="expression"></param>
        public virtual void SetParameterize(Func<Expression, Expression> expression)
        {
            _parameterize = expression;
        }

        /// <summary>
        /// Retrieves all filters for all entity types
        /// </summary>
        public virtual ReadOnlyCollection<TypeFilters> All => _all;

        /// <summary>
        /// Function to parameterize an entity filter expression
        /// </summary>
        public virtual Func<Expression, Expression> Parameterize => _parameterize;
    }
}