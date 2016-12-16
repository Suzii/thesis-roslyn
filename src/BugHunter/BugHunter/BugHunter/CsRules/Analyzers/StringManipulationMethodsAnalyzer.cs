using System.Collections.Immutable;
using BugHunter.Core;
using BugHunter.Core.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    /// <summary>
    /// Searches for usages of 'ToLower()' and 'ToUpper()' methods called on strings and reports their usage when no overload with StringComparison argument is used
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringManipulationMethodsAnalyzer : BaseMemberInvocationAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.STRING_MANIPULATION_METHODS;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
            title: new LocalizableResourceString(nameof(CsResources.StringManipulationMethods_Title), CsResources.ResourceManager, typeof(CsResources)),
            messageFormat: new LocalizableResourceString(nameof(CsResources.StringManipulationMethods_MessageFormat), CsResources.ResourceManager, typeof(CsResources)),
            category: AnalyzerCategories.CS_RULES,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CsResources.StringManipulationMethods_Description), CsResources.ResourceManager, typeof(CsResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);


        protected override string GetForbiddenUsageTextForUserMessage(InvocationExpressionSyntax invocationExpression)
        {
            return $"{MethodInvocationHelper.GetMethodName(invocationExpression)}()";
        }

        protected override Location GetWarningLocation(InvocationExpressionSyntax invocationExpression)
        {
            return LocationHelper.GetLocationOfMethodInvocationOnly(invocationExpression);
        }

        public override void Initialize(AnalysisContext context)
        {
            RegisterAction(Rule, context, "System.String", "ToLower");
            RegisterAction(Rule, context, "System.String", "ToUpper");
        }
    }
}
