using System.Collections.Immutable;
using BugHunter.Core.Analyzers;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Constants;
using BugHunter.Core.DiagnosticsFormatting.Implementation;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Web.Analyzers.CmsApiGuidelinesRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConnectionHelperExecuteQueryAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raised by <see cref="ConnectionHelperExecuteQueryAnalyzer"/>
        /// </summary>
        public const string DIAGNOSTIC_ID = DiagnosticIds.CONNECTION_HELPER_EXECUTE_QUERY;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
                title: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.ConnectionHelperExecuteQuery_Title), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
                messageFormat: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.ConnectionHelperExecuteQuery_MessageFormat), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
                category: nameof(AnalyzerCategories.CmsApiGuidelines),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.ConnectionHelperExecuteQuery_Description), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
                helpLinkUri: HelpLinkUriProvider.GetHelpLink(DIAGNOSTIC_ID));

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
            => ImmutableArray.Create(Rule);

        private static readonly ISyntaxNodeAnalyzer analyzer = new InnerMethodInvocationAnalyzer();

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(analyzer.Run, SyntaxKind.InvocationExpression);
        }

        private sealed class InnerMethodInvocationAnalyzer : MethodInvocationAnalyzer
        {
            private static readonly ApiReplacementConfig config = new ApiReplacementConfig(Rule,
            new[] { "CMS.DataEngine.ConnectionHelper" },
            new[] { "ExecuteQuery" });

            /// <summary>
            /// Constructor initializing config and diagnostic formatter of <see cref="MethodInvocationAnalyzer"/> base class
            /// </summary>
            public InnerMethodInvocationAnalyzer() : base(config, new MethodInvocationDiagnosticFormatter())
            {
            }

            /// <inheritdoc />
            protected override bool IsOnForbiddenPath(string filePath)
            {
                return filePath.EndsWith(FileExtensions.PAGES)
                    || filePath.EndsWith(FileExtensions.CONTROLS)
                    || filePath.EndsWith(FileExtensions.HANDLERS)
                    || filePath.EndsWith(FileExtensions.MASTER_PAGE);
            }
        }
    }
}
