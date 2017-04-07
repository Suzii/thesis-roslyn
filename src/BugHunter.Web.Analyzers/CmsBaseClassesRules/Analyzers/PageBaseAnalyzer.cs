using System.Collections.Immutable;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Helpers.DiagnosticDescriptionBuilders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Web.Analyzers.CmsBaseClassesRules.Analyzers
{
    /// <summary>
    /// Checks if Page file inherits from right class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PageBaseAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.PAGE_BASE;

        private static readonly DiagnosticDescriptor Rule = BaseClassesInheritanceRuleBuilder.GetRule(DIAGNOSTIC_ID, "Page", "some abstract CMSPage");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly ISymbolDiagnosticFormatter<INamedTypeSymbol> DiagnosticFormatter = DiagnosticFormatterFactory.CreateNamedTypeSymbolFormatter();

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSymbolAction(symbolAnalysisContext =>
            {
                var namedTypeSymbol = symbolAnalysisContext.Symbol as INamedTypeSymbol;
                if (namedTypeSymbol == null || namedTypeSymbol.IsAbstract)
                {
                    return;
                }

                var baseTypeSymbol = namedTypeSymbol.BaseType;
                if (baseTypeSymbol == null || !baseTypeSymbol.ToString().Equals("System.Web.UI.Page"))
                {
                    return;
                }

                var diagnostic = DiagnosticFormatter.CreateDiagnostic(Rule, namedTypeSymbol);
                symbolAnalysisContext.ReportDiagnostic(diagnostic);
            }, SymbolKind.NamedType);
        }
    }
}
