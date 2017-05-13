using System.Collections.Immutable;
using BugHunter.Core.Analyzers;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Constants;
using BugHunter.Core.DiagnosticsFormatting.Implementation;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;

namespace BugHunter.Web.Analyzers.CmsApiGuidelinesRules.Analyzers
{
    /// <summary>
    /// Searches for usages of <c>CMS.Helpers.ValidationHelper</c> and their access to <c>GetDouble</c>, <c>GetDateTime</c> methods
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ValidationHelperGetAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raised by <see cref="ValidationHelperGetAnalyzer"/>
        /// </summary>
        public const string DiagnosticId = DiagnosticIds.ValidationHelperGet;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            title: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.ValidationHelperGet_Title), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
            messageFormat: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.ValidationHelperGet_MessageFormat), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
            category: nameof(AnalyzerCategories.CmsApiGuidelines),
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.ValidationHelperGet_Description), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
            helpLinkUri: HelpLinkUriProvider.GetHelpLink(DiagnosticId));

        private static readonly ApiReplacementConfig Config = new ApiReplacementConfig(
            Rule,
            new[] { "CMS.Helpers.ValidationHelper" },
            new[] { "GetDouble", "GetDecimal", "GetDate", "GetDateTime" });

        private static readonly ISyntaxNodeAnalyzer Analyzer = new InnerMethodInvocationAnalyzer(Config);
        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(Analyzer.Run, SyntaxKind.InvocationExpression);
        }

        private sealed class InnerMethodInvocationAnalyzer : MethodInvocationAnalyzer
        {
            /// <summary>
            /// Constructor initializing config and diagnostic formatter of <see cref="MethodInvocationAnalyzer"/> base class
            /// </summary>
            public InnerMethodInvocationAnalyzer(ApiReplacementConfig config)
                : base(config, new MethodInvocationOnlyNoArgsDiagnosticFormatter()) { }

            /// <inheritdoc />
            protected override bool IsOnForbiddenPath(string filePath)
            {
                return SolutionFolders.FileIsInWebPartsFolder(filePath);
            }
        }
    }
}
