using System.Collections.Generic;
using System.Linq;
using BugHunter.Core.DiagnosticFormatting;
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
        private readonly IDiagnosticFormatter _diagnosticFormatter;

        protected BaseMemberAccessAnalyzer()
        {
            _diagnosticFormatter = DiagnosticFormatterFactory.CreateMemberAccessFormatter();
        }

        protected BaseMemberAccessAnalyzer(IDiagnosticFormatter diagnosticFormatter)
        {
            _diagnosticFormatter = diagnosticFormatter;
        }

        protected void RegisterAction(DiagnosticDescriptor rule, AnalysisContext context, string accessedType, string memberName, params string[] additionalMemberNames)
        {
            var forbiddenMemberNames = new[] {memberName}.Concat(additionalMemberNames);

            context.RegisterSyntaxNodeAction(c => Analyze(rule, c, accessedType, forbiddenMemberNames), SyntaxKind.SimpleMemberAccessExpression);
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

            var usedAs = _diagnosticFormatter.GetDiagnosedUsage(memberAccess);
            var location = _diagnosticFormatter.GetLocation(memberAccess);
            var diagnostic = Diagnostic.Create(rule, location, usedAs);

            context.ReportDiagnostic(diagnostic);
        }
    }
}