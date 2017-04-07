using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.DiagnosticDescriptionBuilders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Web.Analyzers.CmsBaseClassesRules.Analyzers
{
    /// <summary>
    /// Checks if Web Part file inherits from right class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WebPartBaseAnalyzer : DiagnosticAnalyzer
    {
        public const string WEB_PART_DIAGNOSTIC_ID = DiagnosticIds.WEB_PART_BASE;
        public const string UI_WEB_PART_DIAGNOSTIC_ID = DiagnosticIds.UI_WEB_PART_BASE;

        private static readonly DiagnosticDescriptor WebPartRule = BaseClassesInheritanceRuleBuilder.GetRule(WEB_PART_DIAGNOSTIC_ID, "Web Part", "some abstract CMS WebPart");
        private static readonly DiagnosticDescriptor UiWebPartRule = BaseClassesInheritanceRuleBuilder.GetRule(UI_WEB_PART_DIAGNOSTIC_ID, "UI Web Part", "some abstract CMS UI WebPart");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(WebPartRule, UiWebPartRule);

        private static readonly ISymbolDiagnosticFormatter<INamedTypeSymbol> DiagnosticFormatter = DiagnosticFormatterFactory.CreateNamedTypeSymbolFormatter();

        private static readonly string[] UiWebPartBases =
        {
            "CMS.UIControls.CMSAbstractUIWebpart"
        };

        private static readonly string[] WebPartBases =
        {
            "CMS.PortalEngine.Web.UI.CMSAbstractWebPart",
            "CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart",
            "CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart",
            "CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart",
            "CMS.Ecommerce.Web.UI.CMSCheckoutWebPart"
        };


        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSymbolAction(Analyze, SymbolKind.NamedType);
        }

        private static void Analyze(SymbolAnalysisContext symbolAnalysisContext)
        {
            var namedTypeSymbol = (INamedTypeSymbol)symbolAnalysisContext.Symbol;
            if (namedTypeSymbol == null || 
                namedTypeSymbol.IsAbstract || 
                namedTypeSymbol.IsNested())
            {
                return;
            }

            // Symbol is defined in multiple locations if it is a partial class -- all locations need to be checked
            var symbolFilePaths = namedTypeSymbol.Locations.Select(location => location?.SourceTree?.FilePath).ToArray();
            var isInWebPartsFolder = symbolFilePaths.Any(SolutionFolders.FileIsInWebPartsFolder);
            if (!isInWebPartsFolder)
            {
                return;
            }

            var isUiWebPart = symbolFilePaths.Any(SolutionFolders.IsUIWebPart);
            var allowedBaseTypeNames = isUiWebPart ? UiWebPartBases : WebPartBases;
            var ruleToBeUsed = isUiWebPart ? UiWebPartRule : WebPartRule;
            
            var extendsOnlyObject = namedTypeSymbol.ExtendsOnlyObject();
            var inheritsFromOneOfRequiredTypes = InheritsFromOneOfRequiredTypes(allowedBaseTypeNames, namedTypeSymbol);
            if (extendsOnlyObject || inheritsFromOneOfRequiredTypes)
            {
                return;
            }

            var diagnostic = DiagnosticFormatter.CreateDiagnostic(ruleToBeUsed, namedTypeSymbol);
            symbolAnalysisContext.ReportDiagnostic(diagnostic);
        }

        private static bool InheritsFromOneOfRequiredTypes(string[] allowedBaseTypeNames, INamedTypeSymbol namedTypeSymbol)
        {
            return allowedBaseTypeNames
                .Any(allowedTypeName => allowedTypeName.Equals(namedTypeSymbol.BaseType?.ToString()));
        }
    }
}
