using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core;
using BugHunter.Core.Analyzers;
using BugHunter.Core.DiagnosticsFormatting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiGuidelinesRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EventLogArgumentsAnalyzer : BaseMemberInvocationAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.EVENT_LOG_ARGUMENTS;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
            title: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.EventLogArguments_Title), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
            messageFormat: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.EventLogArguments_MessageFormat), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
            category: nameof(AnalyzerCategories.CmsApiGuidelines),
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.EventLogArguments_Description), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)));

        private static readonly IDiagnosticFormatter<InvocationExpressionSyntax> _diagnosticFormatter = new EventLogArgumentsDiagnosticFormatter();

        private static readonly string[] ForbiddenEventTypeArgs = { "\"I\"", "\"W\"", "\"E\"" };

        protected override IDiagnosticFormatter<InvocationExpressionSyntax> DiagnosticFormatter => _diagnosticFormatter;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            RegisterAction(Rule, context, "CMS.EventLog.EventLogProvider", "LogEvent");
        }

        /// <summary>
        /// Checks whether first argument provided is one of forbidden ones
        /// 
        /// Note that analysis has to be performed on syntax since it always is a string, but we want to achieve that not plain string is passed but rather a constant defined in CMS.EventLog.EventType
        /// TODO Known limitations: 
        /// When a variable is passed, there is no way of finding out whether the variable was created from a plain string or an constant defined in EventType class (it is not an enum),
        /// therefore no diagnostic is provided
        /// </summary>
        /// <param name="context">Analysis context</param>
        /// <param name="invocationExpression">Invocation expression to be analyzed</param>
        /// <param name="methodSymbol">Method symbol for <param name="invocationExpression"></param></param>
        /// <returns>Tru if forbidden usage is detected, false otherwise</returns>
        protected override bool CheckPostConditions(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationExpression, IMethodSymbol methodSymbol)
        {
            if (invocationExpression.ArgumentList.Arguments.Count == 0)
            {
                return false;
            }

            var eventTypeArgument = invocationExpression.ArgumentList.Arguments.First();
            var firstArgumentText = eventTypeArgument.Expression.ToString();

            return ForbiddenEventTypeArgs.Contains(firstArgumentText);
        }
    }
}
