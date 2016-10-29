using System.Collections.Immutable;
using BugHunter.Core.Extensions;
using BugHunter.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpSessionElementAccessAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID_GET = DiagnosticIds.HTTP_SESSION_ELEMENT_ACCESS_GET;
        public const string DIAGNOSTIC_ID_SET = DiagnosticIds.HTTP_SESSION_ELEMENT_ACCESS_SET;

        private static readonly DiagnosticDescriptor RuleForGet = new DiagnosticDescriptor(DIAGNOSTIC_ID_GET,
            title: new LocalizableResourceString(nameof(CsResources.HttpSessionElementAccessGet_Title), CsResources.ResourceManager, typeof(CsResources)),
            messageFormat: new LocalizableResourceString(nameof(CsResources.HttpSessionElementAccessGet_MessageFormat), CsResources.ResourceManager, typeof(CsResources)),
            category: AnalyzerCategories.CS_RULES,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CsResources.HttpSessionElementAccessGet_Description), CsResources.ResourceManager, typeof(CsResources)));

        private static readonly DiagnosticDescriptor RuleForSet = new DiagnosticDescriptor(DIAGNOSTIC_ID_SET,
            title: new LocalizableResourceString(nameof(CsResources.HttpSessionElementAccessSet_Title), CsResources.ResourceManager, typeof(CsResources)),
            messageFormat: new LocalizableResourceString(nameof(CsResources.HttpSessionElementAccessSet_MessageFormat), CsResources.ResourceManager, typeof(CsResources)),
            category: AnalyzerCategories.CS_RULES,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CsResources.HttpSessionElementAccessSet_Description), CsResources.ResourceManager, typeof(CsResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(RuleForGet, RuleForSet);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ElementAccessExpression);
        }

        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            var compilation = context.SemanticModel.Compilation;
            var elementAccess = (ElementAccessExpressionSyntax)context.Node;
            var accessedTypeSymbol = context.SemanticModel.GetTypeInfo(elementAccess.Expression).Type as INamedTypeSymbol;
            
            if (accessedTypeSymbol == null || (!IsHttpSession(accessedTypeSymbol, compilation) && !IsHttpSessionBase(accessedTypeSymbol, compilation)))
            {
                return;

            }
            
            if (IsUsedForAssignment(elementAccess))
            {
                var assignmentExpression = elementAccess.FirstAncestorOrSelf<AssignmentExpressionSyntax>();
                var diagnostic = Diagnostic.Create(RuleForSet, assignmentExpression.GetLocation(), elementAccess);
                context.ReportDiagnostic(diagnostic);
            }
            else
            {
                var diagnostic = Diagnostic.Create(RuleForGet, elementAccess.GetLocation(), elementAccess);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool IsHttpSession(INamedTypeSymbol accessedTypeSymbol, Compilation compilation)
        {
            var sessionType = typeof(System.Web.SessionState.HttpSessionState);
            var sessionTypeSymbol = sessionType.GetITypeSymbol(compilation);

            return accessedTypeSymbol.IsDerivedFromClassOrInterface(sessionTypeSymbol);
        }

        private static bool IsHttpSessionBase(INamedTypeSymbol accessedTypeSymbol, Compilation compilation)
        {
            var sessionBaseType = typeof(System.Web.HttpSessionStateBase);
            var sessionBaseTypeSymbol = sessionBaseType.GetITypeSymbol(compilation);

            return accessedTypeSymbol.IsDerivedFromClassOrInterface(sessionBaseTypeSymbol);
        }

        private bool IsUsedForAssignment(ElementAccessExpressionSyntax elementAccess)
        {
            // look for parent of type SimpleAssignmentexpressionSyntax and make sure elementAccess is an lvalue of this assignment
            var assignmentExpression = elementAccess.FirstAncestorOrSelf<AssignmentExpressionSyntax>();

            return assignmentExpression?.Left.Contains(elementAccess) ?? false;
        }
    }
}
