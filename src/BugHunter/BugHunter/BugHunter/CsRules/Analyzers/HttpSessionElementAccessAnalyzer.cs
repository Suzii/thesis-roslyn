using System.Collections.Immutable;
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
        public const string DIAGNOSTIC_ID = DiagnosticIds.HTTP_SESSION_ELEMENT_ACCESS;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
            title: new LocalizableResourceString(nameof(CsResources.HttpSessionElementAccess_Title), CsResources.ResourceManager, typeof(CsResources)),
            messageFormat: new LocalizableResourceString(nameof(CsResources.HttpSessionElementAccess_MessageFormat), CsResources.ResourceManager, typeof(CsResources)),
            category: AnalyzerCategories.CS_RULES,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CsResources.HttpSessionElementAccess_Description), CsResources.ResourceManager, typeof(CsResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ElementAccessExpression);
        }

        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            var elementAccess = (ElementAccessExpressionSyntax)context.Node;

            var accessedTypeSymbol = context.SemanticModel.GetTypeInfo(elementAccess.Expression).Type as INamedTypeSymbol;
            
            var compilation = context.SemanticModel.Compilation;
            if (accessedTypeSymbol == null || (!IsHttpSession(accessedTypeSymbol, compilation) && !IsHttpSessionBase(accessedTypeSymbol, compilation)))
            {
                return;

            }

            var diagnostic = Diagnostic.Create(Rule, elementAccess.GetLocation(), elementAccess);
            context.ReportDiagnostic(diagnostic);
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
    }
}
