using System.Collections.Generic;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core.Analyzers
{
    /// <summary>
    /// Used for analysis of SimpleMemberAccess such as "page.IsCallback"
    /// </summary>
    public abstract class BaseMemberAccessAnalyzer : BaseMemberAccessOrInvocationAnalyzer<MemberAccessExpressionSyntax>
    {
        private static readonly IDiagnosticFormatter _diagnosticFormatter = DiagnosticFormatterFactory.CreateMemberAccessFormatter();

        protected override IDiagnosticFormatter DiagnosticFormatter => _diagnosticFormatter;

        protected void RegisterAction(DiagnosticDescriptor rule, AnalysisContext context, string accessedType, string memberName, params string[] additionalMemberNames)
        {
            var forbiddenMemberNames = new HashSet<string>(additionalMemberNames) { memberName };

            context.RegisterSyntaxNodeAction(c => Analyze(rule, c, accessedType, forbiddenMemberNames), SyntaxKind.SimpleMemberAccessExpression);
        }

        protected override bool CheckMainConditions(SyntaxNodeAnalysisContext context, string accessedType, ISet<string> memberNames)
        {
            var memberAccess = (MemberAccessExpressionSyntax) context.Node;
            if (memberAccess == null)
            {
                return false;
            }

            var memberName = memberAccess.Name.ToString();
            if (!memberNames.Contains(memberName))
            {
                return false;
            }

            var searchedTargetType = context.SemanticModel.Compilation.GetTypeByMetadataName(accessedType);
            var actualTargetType = context.SemanticModel.GetTypeInfo(memberAccess.Expression).Type as INamedTypeSymbol;
            if (searchedTargetType == null || actualTargetType == null)
            {
                return false;
            }

            return actualTargetType.IsDerivedFromClassOrInterface(searchedTargetType);
        }
    }
}