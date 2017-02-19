using System.Collections.Immutable;
using BugHunter.Core;
using BugHunter.Core.Analyzers;
using BugHunter.Core.DiagnosticsFormatting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiGuidelinesRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WhereLikeMethodAnalyzer : BaseMemberInvocationAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.WHERE_LIKE_METHOD;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
            title: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.WhereLikeMethod_Title), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
            messageFormat: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.WhereLikeMethod_MessageFormat), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
            category: nameof(AnalyzerCategories.AbstractionOverImplementation),
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.WhereLikeMethod_Description), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)));

        private static readonly IDiagnosticFormatter _diagnosticFormatter = DiagnosticFormatterFactory.CreateMemberInvocationOnlyFormatter();

        protected override IDiagnosticFormatter DiagnosticFormatter => _diagnosticFormatter;

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            var accessedType = "CMS.DataEngine.WhereConditionBase`1";
            RegisterAction(Rule, context, accessedType, "WhereLike", "WhereNotLike");
        }
    }
}
