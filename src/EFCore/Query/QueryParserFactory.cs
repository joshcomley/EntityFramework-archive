using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.ResultOperators.Internal;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Parsing.Structure.ExpressionTreeProcessors;
using Remotion.Linq.Parsing.Structure.NodeTypeProviders;

namespace Microsoft.EntityFrameworkCore.Query
{

    /// <summary>
    /// Factory to create instances of <see cref="QueryParser"/>
    /// </summary>
    public class QueryParserFactory : IQueryParserFactory
    {
        private readonly MethodInfoBasedNodeTypeRegistry _methodInfoBasedNodeTypeRegistry;
        private INodeTypeProvider _nodeTypeProvider;

        /// <summary>
        ///     Creates a new instance of the <see cref="QueryParserFactory"/>.
        /// </summary>
        /// <param name="methodInfoBasedNodeTypeRegistry"></param>
        public QueryParserFactory(
            [NotNull] MethodInfoBasedNodeTypeRegistry methodInfoBasedNodeTypeRegistry
            )
        {
            _methodInfoBasedNodeTypeRegistry = methodInfoBasedNodeTypeRegistry;
        }

        private INodeTypeProvider NodeTypeProvider
            => _nodeTypeProvider
               ?? (_nodeTypeProvider
                   = QueryParserFactory.CreateNodeTypeProvider(_methodInfoBasedNodeTypeRegistry));

        internal static readonly IEvaluatableExpressionFilter EvaluatableExpressionFilter
            = new EvaluatableExpressionFilterImpl();

        /// <summary>
        ///     Creates an instance of the <see cref="QueryParser"/> class.
        /// </summary>
        /// <returns></returns>
        public virtual QueryParser CreateQueryParser()
            => new QueryParser(
                new ExpressionTreeParser(
                    NodeTypeProvider,
                    new CompoundExpressionTreeProcessor(new IExpressionTreeProcessor[]
                    {
                        new PartialEvaluatingExpressionTreeProcessor(EvaluatableExpressionFilter),
                        new TransformingExpressionTreeProcessor(ExpressionTransformerRegistry.CreateDefault())
                    })));

        private class EvaluatableExpressionFilterImpl : EvaluatableExpressionFilterBase
        {
            private static readonly PropertyInfo _dateTimeNow
                = typeof(DateTime).GetTypeInfo().GetDeclaredProperty(nameof(DateTime.Now));

            private static readonly PropertyInfo _dateTimeUtcNow
                = typeof(DateTime).GetTypeInfo().GetDeclaredProperty(nameof(DateTime.UtcNow));

            private static readonly MethodInfo _guidNewGuid
                = typeof(Guid).GetTypeInfo().GetDeclaredMethod(nameof(Guid.NewGuid));

            private static readonly List<MethodInfo> _randomNext
                = typeof(Random).GetTypeInfo().GetDeclaredMethods(nameof(Random.Next)).ToList();

            public override bool IsEvaluatableMethodCall(MethodCallExpression methodCallExpression)
            {
                if ((methodCallExpression.Method == _guidNewGuid)
                    || _randomNext.Contains(methodCallExpression.Method))
                {
                    return false;
                }

                return base.IsEvaluatableMethodCall(methodCallExpression);
            }

            public override bool IsEvaluatableMember(MemberExpression memberExpression)
                => memberExpression.Member != _dateTimeNow && memberExpression.Member != _dateTimeUtcNow;
        }

        internal static INodeTypeProvider CreateNodeTypeProvider(
            MethodInfoBasedNodeTypeRegistry methodInfoBasedNodeTypeRegistry)
        {
            methodInfoBasedNodeTypeRegistry
                .Register(TrackingExpressionNode.SupportedMethods, typeof(TrackingExpressionNode));

            methodInfoBasedNodeTypeRegistry
                .Register(IncludeExpressionNode.SupportedMethods, typeof(IncludeExpressionNode));

            methodInfoBasedNodeTypeRegistry
                .Register(ThenIncludeExpressionNode.SupportedMethods, typeof(ThenIncludeExpressionNode));

            var innerProviders
                = new INodeTypeProvider[]
                {
                    methodInfoBasedNodeTypeRegistry,
                    MethodNameBasedNodeTypeRegistry.CreateFromRelinqAssembly()
                };

            return new CompoundNodeTypeProvider(innerProviders);
        }
    }
}