using Remotion.Linq.Parsing.Structure;

namespace Microsoft.EntityFrameworkCore.Query
{
    /// <summary>
         /// Factory to create instances of <see cref="QueryParser"/>
         /// </summary>
    public interface IQueryParserFactory
    {
        /// <summary>
        ///     Create an instance of <see cref="QueryParser"/>
        /// </summary>
        /// <returns></returns>
        QueryParser CreateQueryParser();
    }
}