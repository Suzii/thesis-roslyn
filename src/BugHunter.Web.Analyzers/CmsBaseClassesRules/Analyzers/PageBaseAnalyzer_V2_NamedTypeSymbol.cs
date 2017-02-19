using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core.Analyzers;
using BugHunter.Core.Helpers.DiagnosticDescriptionBuilders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Web.Analyzers.CmsBaseClassesRules.Analyzers
{
    /// <summary>
    /// Checks if Page file inherits from right class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PageBaseAnalyzer_V2_NamedTypeSymbol : BaseClassDeclarationSyntaxAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.PAGE_BASE;

        private static readonly DiagnosticDescriptor Rule = BaseClassesInheritanceRuleBuilder.GetRule(DIAGNOSTIC_ID, "Page", "some abstract CMSPage");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(compilationContext =>
            {
                var systemWebUiPageType = compilationContext.Compilation.GetTypeByMetadataName("System.Web.UI.Page");
                if (systemWebUiPageType == null)
                {
                    return;
                }

                compilationContext.RegisterSymbolAction(symbolAnalysisContext =>
                {
                    var namedTypeSymbol = symbolAnalysisContext.Symbol as INamedTypeSymbol;
                    if (namedTypeSymbol == null || namedTypeSymbol.IsAbstract)
                    {
                        return;
                    }

                    var baseTypeSymbol = namedTypeSymbol.BaseType;
                    if (baseTypeSymbol == null || !baseTypeSymbol.Equals(systemWebUiPageType))
                    {
                        return;
                    }

                    var location = namedTypeSymbol.Locations.FirstOrDefault();
                    var diagnostic = Diagnostic.Create(Rule, location, namedTypeSymbol.Name.ToString());
                    symbolAnalysisContext.ReportDiagnostic(diagnostic);
                }, SymbolKind.NamedType);
            });
        }
    }
}
