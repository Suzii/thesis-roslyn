using System.Collections.Immutable;
using BugHunter.Core;
using BugHunter.Core.Analyzers;
using BugHunter.Core.DiagnosticsFormatting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.InternalGuidelinesRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WhereLikeMethodAnalyzer : BaseMemberInvocationAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.WHERE_LIKE_METHOD;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
            title: new LocalizableResourceString(nameof(InternalGuidelinesResources.WhereLikeMethod_Title), InternalGuidelinesResources.ResourceManager, typeof(InternalGuidelinesResources)),
            messageFormat: new LocalizableResourceString(nameof(InternalGuidelinesResources.WhereLikeMethod_MessageFormat), InternalGuidelinesResources.ResourceManager, typeof(InternalGuidelinesResources)),
            category: AnalyzerCategories.AbstractionOverImplementation,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(InternalGuidelinesResources.WhereLikeMethod_Description), InternalGuidelinesResources.ResourceManager, typeof(InternalGuidelinesResources)));
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            var accessedType = "CMS.DataEngine.WhereConditionBase`1";
            RegisterAction(Rule, context, accessedType, "WhereLike", "WhereNotLike");
        }

        protected override IDiagnosticFormatter GetDiagnosticFormatter()
        {
            return DiagnosticFormatterFactory.CreateMemberInvocationOnlyFormatter();
        }
    }
}
