using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.AnalysisHelpers
{
    public class AccessExpressionHelper
    {
        public ITypeSymbol GetTargetType(SemanticModel semanticModel, MemberAccessExpressionSyntax accessExpressionSyntax)
        {
            return semanticModel.GetTypeInfo(accessExpressionSyntax.Expression).Type;
        }

        public ITypeSymbol GetTargetType(SemanticModel semanticModel, ConditionalAccessExpressionSyntax accessExpressionSyntax)
        {
            return semanticModel.GetTypeInfo(accessExpressionSyntax.Expression).Type;
        }
    }
}