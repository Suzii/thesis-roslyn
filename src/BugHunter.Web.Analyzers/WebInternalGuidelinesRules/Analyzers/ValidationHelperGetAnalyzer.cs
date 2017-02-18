using System.Collections.Immutable;
using BugHunter.Core;
using BugHunter.Core.Analyzers;
using BugHunter.Core.DiagnosticsFormatting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Web.Analyzers.WebInternalGuidelinesRules.Analyzers
{
    /// <summary>
    /// Searches for usages of <c>CMS.Helpers.ValidationHelper</c> and their access to <c>GetDouble</c>, <c>GetDateTime</c> methods
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ValidationHelperGetAnalyzer : BaseMemberAccessAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.VALIDATION_HELPER_GET;
        
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
                title: new LocalizableResourceString(nameof(WebInternalGuidelinesResources.ValidationHelperGet_Title), WebInternalGuidelinesResources.ResourceManager, typeof(WebInternalGuidelinesResources)),
                messageFormat: new LocalizableResourceString(nameof(WebInternalGuidelinesResources.ValidationHelperGet_MessageFormat), WebInternalGuidelinesResources.ResourceManager, typeof(WebInternalGuidelinesResources)),
                category: nameof(AnalyzerCategories.WebInternalGuidelines),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(WebInternalGuidelinesResources.ValidationHelperGet_Description), WebInternalGuidelinesResources.ResourceManager, typeof(WebInternalGuidelinesResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            RegisterAction(Rule, context, "CMS.Helpers.ValidationHelper", "GetDouble", "GetDecimal", "GetDate", "GetDateTime");
        }

        protected override IDiagnosticFormatter GetDiagnosticFormatter()
        {
            return DiagnosticFormatterFactory.CreateMemberAccessOnlyFormatter();
        }
    }
}
