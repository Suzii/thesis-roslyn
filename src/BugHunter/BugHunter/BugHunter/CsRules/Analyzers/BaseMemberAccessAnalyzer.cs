using System.Collections.Generic;
using System.Linq;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    /// <summary>
    /// Used for analysis of SimpleMemberAccess such as "page.IsCallback"
    /// </summary>
    public abstract class BaseMemberAccessAnalyzer : DiagnosticAnalyzer
    {
        protected void RegisterAction(DiagnosticDescriptor rule, AnalysisContext context, string accessedType, string memberName, params string[] additionalMemberNames)
        {
            var forbiddenMemberNames = new[] {memberName}.Concat(additionalMemberNames);

            context.RegisterSyntaxNodeAction(c => Analyze(rule, c, accessedType, forbiddenMemberNames), SyntaxKind.SimpleMemberAccessExpression);
        }

        protected virtual  Location GetWarningLocation(MemberAccessExpressionSyntax memberAccess)
        {
            var location = memberAccess.GetLocation();

            return location;
        }

        protected virtual string GetForbiddenUsageTextForUserMessage(MemberAccessExpressionSyntax memberAccess)
        {
            var usedAs = $"{memberAccess.Expression}.{memberAccess.Name}";

            return usedAs;
        }

        private void Analyze(DiagnosticDescriptor rule, SyntaxNodeAnalysisContext context, string accessedType, IEnumerable<string> forbiddenMemberNames)
        {
            var memberAccess = (MemberAccessExpressionSyntax)context.Node;

            var memberName = memberAccess.Name.ToString();
            if (!forbiddenMemberNames.Contains(memberName))
            {
                return;
            }

            var searchedTargetType = TypeExtensions.GetITypeSymbol(accessedType, context);
            var actualTargetType = new SemanticModelBrowser(context).GetMemberAccessTarget(memberAccess) as INamedTypeSymbol;
            if (searchedTargetType == null || actualTargetType == null || !actualTargetType.IsDerivedFromClassOrInterface(searchedTargetType))
            {
                return;
            }

            var usedAs = GetForbiddenUsageTextForUserMessage(memberAccess);
            var location = GetWarningLocation(memberAccess);
            var diagnostic = Diagnostic.Create(rule, location, usedAs);

            context.ReportDiagnostic(diagnostic);
        }
    }
}