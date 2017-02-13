using System.Collections.Immutable;
using BugHunter.Core;
using BugHunter.Core.Analyzers;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WhereLikeMethodAnalyzer : BaseMemberInvocationAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.WHERE_LIKE_METHOD;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
            title: new LocalizableResourceString(nameof(CsResources.WhereLikeMethod_Title), CsResources.ResourceManager, typeof(CsResources)),
            messageFormat: new LocalizableResourceString(nameof(CsResources.WhereLikeMethod_MessageFormat), CsResources.ResourceManager, typeof(CsResources)),
            category: AnalyzerCategories.AbstractionOverImplementation,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CsResources.WhereLikeMethod_Description), CsResources.ResourceManager, typeof(CsResources)));
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
