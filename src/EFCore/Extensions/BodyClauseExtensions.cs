// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Remotion.Linq.Clauses;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    internal static class BodyClauseExtensions
    {
        /// <summary>
        /// Ensure that any "additional" clauses come last as we rely on ordering later on
        /// </summary>
        /// <param name="bodyClauses"></param>
        public static void Sort(this ICollection<IBodyClause> bodyClauses)
        {
            var clauses = bodyClauses.ToList().OrderBy(clause => clause is AdditionalFromClause);
            bodyClauses.Clear();
            foreach (var clause in clauses)
            {
                bodyClauses.Add(clause);
            }
        }
    }
}