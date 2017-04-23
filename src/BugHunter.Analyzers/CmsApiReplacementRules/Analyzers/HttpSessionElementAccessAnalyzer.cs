using System.Collections.Immutable;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiReplacementRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpSessionElementAccessAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID_GET = DiagnosticIds.HTTP_SESSION_ELEMENT_ACCESS_GET;

        public const string DIAGNOSTIC_ID_SET = DiagnosticIds.HTTP_SESSION_ELEMENT_ACCESS_SET;

        private static readonly DiagnosticDescriptor RuleForGet = ApiReplacementRulesProvider.GetRule(DIAGNOSTIC_ID_GET, "Session[]", "SessionHelper.GetValue()");

        private static readonly DiagnosticDescriptor RuleForSet = ApiReplacementRulesProvider.GetRule(DIAGNOSTIC_ID_SET, "Session[]", "SessionHelper.SetValue()");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(RuleForGet, RuleForSet);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ElementAccessExpression);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var elementAccess = (ElementAccessExpressionSyntax)context.Node;
            if (elementAccess == null)
            {
                return;
            }

            var compilation = context.SemanticModel.Compilation;
            var accessedTypeSymbol = context.SemanticModel.GetTypeInfo(elementAccess.Expression).Type as INamedTypeSymbol;
            if (accessedTypeSymbol == null || (!IsHttpSession(accessedTypeSymbol, compilation) && !IsHttpSessionBase(accessedTypeSymbol, compilation)))
            {
                return;
            }

            var diagnostic = GetDiagnostic(elementAccess);
            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsHttpSession(INamedTypeSymbol accessedTypeSymbol, Compilation compilation)
            => accessedTypeSymbol?.IsDerivedFrom("System.Web.SessionState.HttpSessionState", compilation) ?? false;

        private static bool IsHttpSessionBase(INamedTypeSymbol accessedTypeSymbol, Compilation compilation)
            => accessedTypeSymbol?.IsDerivedFrom("System.Web.HttpSessionStateBase", compilation) ?? false;

        private static Diagnostic GetDiagnostic(ElementAccessExpressionSyntax elementAccess)
        {
            if (!elementAccess.IsBeingAssigned())
            {
                return Diagnostic.Create(RuleForGet, elementAccess.GetLocation(), elementAccess);
            }

            var assignmentExpression = elementAccess.FirstAncestorOrSelf<AssignmentExpressionSyntax>();
            return Diagnostic.Create(RuleForSet, assignmentExpression.GetLocation(), elementAccess);
        }
    }
}
