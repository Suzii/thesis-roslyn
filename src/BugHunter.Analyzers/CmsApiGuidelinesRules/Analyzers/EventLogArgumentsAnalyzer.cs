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

        private static readonly IDiagnosticFormatter _diagnosticFormatter = new EventLogArgumentsDiagnosticFormatter();

        protected override IDiagnosticFormatter DiagnosticFormatter => _diagnosticFormatter;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

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
    }
}
