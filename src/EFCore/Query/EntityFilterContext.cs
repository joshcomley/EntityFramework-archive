using System;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Query
{
    /// <summary>
    ///     The context of an entity filter
    /// </summary>
    public class EntityFilterContext
    {
        /// <summary>
        ///     The service provider to use for this filter
        /// </summary>
        public virtual IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Initializes a new instance of the entity filter context
        /// </summary>
        /// <param name="serviceProvider"></param>
        public EntityFilterContext([NotNull]IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}