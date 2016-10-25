using System.Collections.Immutable;
using System.Linq;
using BugHunter.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EventLogArgumentsAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.EventLogArguments;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
            title: new LocalizableResourceString(nameof(CsResources.EventLogArguments_Title), CsResources.ResourceManager, typeof(CsResources)),
            messageFormat: new LocalizableResourceString(nameof(CsResources.EventLogArguments_MessageFormat), CsResources.ResourceManager, typeof(CsResources)),
            category: AnalyzerCategories.CsRules,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CsResources.EventLogArguments_Description), CsResources.ResourceManager, typeof(CsResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
        }

        // TODO what if LogEvent is only passed as method group?
        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;
            var memberAccess = invocationExpression.Expression as MemberAccessExpressionSyntax;
            var forbiddenMemberName = nameof(CMS.EventLog.EventLogProvider.LogEvent);
            // TODO should check also only for invocation (usage in class where defined)?
            if (memberAccess == null || memberAccess.Name.ToString() != forbiddenMemberName)
            {
                return;
            }

            var symbol = context.SemanticModel.GetSymbolInfo(memberAccess).Symbol;
            var memberSymbol = symbol as IMethodSymbol;
            if (memberSymbol == null)
            {
                return;
            }

            var searchedType = typeof(CMS.EventLog.EventLogProvider).GetITypeSymbol(context.SemanticModel.Compilation);
            var actualType = context.SemanticModel.GetTypeInfo(memberAccess.Expression).Type as INamedTypeSymbol;
            if (actualType == null || !actualType.IsDerivedFromClassOrInterface(searchedType))
            {
                return;
            }

            if (invocationExpression.ArgumentList.Arguments.Count == 0)
            {
                return;
            }

            var forbiddenEventTypeArgs = new[] { "\"I\"", "\"W\"", "\"E\"" };
            var eventTypeArgument = invocationExpression.ArgumentList.Arguments.First();
            var firstArgumentText = eventTypeArgument.Expression.ToString();
            if (!forbiddenEventTypeArgs.Contains(firstArgumentText))
            {
                return;
            }

            var warningLocation = invocationExpression.ArgumentList.Arguments.First().GetLocation();
            var diagnostic = Diagnostic.Create(Rule, warningLocation, eventTypeArgument.ToString());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
