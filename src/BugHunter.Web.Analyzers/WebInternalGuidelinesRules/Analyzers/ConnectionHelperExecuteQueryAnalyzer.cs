using System.Collections.Immutable;
using BugHunter.Core;
using BugHunter.Core.Analyzers;
using BugHunter.Core.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Web.Analyzers.WebInternalGuidelinesRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConnectionHelperExecuteQueryAnalyzer : BaseMemberAccessAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.CONNECTION_HELPER_EXECUTE_QUERY;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
                title: new LocalizableResourceString(nameof(WebInternalGuidelinesResources.ConnectionHelperExecuteQuery_Title), WebInternalGuidelinesResources.ResourceManager, typeof(WebInternalGuidelinesResources)),
                messageFormat: new LocalizableResourceString(nameof(WebInternalGuidelinesResources.ConnectionHelperExecuteQuery_MessageFormat), WebInternalGuidelinesResources.ResourceManager, typeof(WebInternalGuidelinesResources)),
                category: nameof(AnalyzerCategories.WebInternalGuidelines),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(WebInternalGuidelinesResources.ConnectionHelperExecuteQuery_Description), WebInternalGuidelinesResources.ResourceManager, typeof(WebInternalGuidelinesResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            RegisterAction(Rule, context, "CMS.DataEngine.ConnectionHelper", "ExecuteQuery");
        }

        protected override bool CheckPreConditions(SyntaxNodeAnalysisContext context)
        {
            return base.CheckPreConditions(context) && IsUiFile(context.Node.SyntaxTree.FilePath);
        }

        private bool IsUiFile(string filePath)
        {
            return filePath.EndsWith(FileExtensions.PAGES) 
                || filePath.EndsWith(FileExtensions.CONTROLS) 
                || filePath.EndsWith(FileExtensions.HANDLERS)
                || filePath.EndsWith(FileExtensions.MASTER_PAGE);
        }
    }
}
