using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core;
using BugHunter.Core.Analyzers;
using BugHunter.Core.DiagnosticsFormatting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.InternalGuidelinesRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EventLogArgumentsAnalyzer : BaseMemberInvocationAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.EVENT_LOG_ARGUMENTS;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
            title: new LocalizableResourceString(nameof(InternalGuidelinesResources.EventLogArguments_Title), InternalGuidelinesResources.ResourceManager, typeof(InternalGuidelinesResources)),
            messageFormat: new LocalizableResourceString(nameof(InternalGuidelinesResources.EventLogArguments_MessageFormat), InternalGuidelinesResources.ResourceManager, typeof(InternalGuidelinesResources)),
            category: nameof(AnalyzerCategories.InternalGuidelines),
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(InternalGuidelinesResources.EventLogArguments_Description), InternalGuidelinesResources.ResourceManager, typeof(InternalGuidelinesResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            RegisterAction(Rule, context, "CMS.EventLog.EventLogProvider", "LogEvent");
        }

        protected override bool CheckPostConditions(SyntaxNodeAnalysisContext expression, InvocationExpressionSyntax invocationExpression)
        {
            if (invocationExpression.ArgumentList.Arguments.Count == 0)
            {
                return false;
            }

            var forbiddenEventTypeArgs = new[] { "\"I\"", "\"W\"", "\"E\"" };
            var eventTypeArgument = invocationExpression.ArgumentList.Arguments.First();
            var firstArgumentText = eventTypeArgument.Expression.ToString();

            return forbiddenEventTypeArgs.Contains(firstArgumentText);
        }

        protected override IDiagnosticFormatter GetDiagnosticFormatter()
        {
            return new EventLogArgumentsDiagnosticFormatter();
        }
    }
}
