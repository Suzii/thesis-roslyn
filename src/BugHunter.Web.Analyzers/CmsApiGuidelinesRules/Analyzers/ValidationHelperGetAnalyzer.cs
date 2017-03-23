using System;
using System.Collections.Immutable;
using BugHunter.Core;
using BugHunter.Core.Analyzers;
using BugHunter.Core.DiagnosticsFormatting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Web.Analyzers.CmsApiGuidelinesRules.Analyzers
{
    /// <summary>
    /// Searches for usages of <c>CMS.Helpers.ValidationHelper</c> and their access to <c>GetDouble</c>, <c>GetDateTime</c> methods
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ValidationHelperGetAnalyzer : BaseMemberInvocationAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.VALIDATION_HELPER_GET;
        
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
                title: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.ValidationHelperGet_Title), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
                messageFormat: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.ValidationHelperGet_MessageFormat), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
                category: nameof(AnalyzerCategories.CmsApiGuidelines),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.ValidationHelperGet_Description), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly IDiagnosticFormatter<InvocationExpressionSyntax> _diagnosticFormatter = DiagnosticFormatterFactory.CreateMemberInvocationOnlyFormatter(true);

        protected override IDiagnosticFormatter<InvocationExpressionSyntax> DiagnosticFormatter => _diagnosticFormatter;

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            RegisterAction(Rule, context, "CMS.Helpers.ValidationHelper",
                "GetDouble", 
                "GetDecimal", 
                "GetDate", 
                "GetDateTime");
        }
        
        protected override bool CheckPreConditions(SyntaxNodeAnalysisContext context)
        {
            return base.CheckPreConditions(context) && FileIsInWebPartsFolder(context.Node.SyntaxTree.FilePath);
        }

        private static bool FileIsInWebPartsFolder(string filePath)
        {
            return !string.IsNullOrEmpty(filePath) &&
                   !filePath.Contains("_files\\") &&
                   (filePath.Contains(SolutionFolders.UI_WEB_PARTS, StringComparison.OrdinalIgnoreCase) || filePath.Contains(SolutionFolders.WEB_PARTS, StringComparison.OrdinalIgnoreCase));
        }
    }
}
