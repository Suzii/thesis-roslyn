using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BH1001EventLogArguments : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = "BH1001";
        private const string CATEGORY = AnalyzerCategories.CsRules;

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(CsResources.BH1001_Title), CsResources.ResourceManager, typeof(CsResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(CsResources.BH1001_MessageFormat), CsResources.ResourceManager, typeof(CsResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(CsResources.BH1001_Description), CsResources.ResourceManager, typeof(CsResources));

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID, Title, MessageFormat, CATEGORY, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;
            var memberAccess = invocationExpression.Expression as MemberAccessExpressionSyntax;
            if (memberAccess == null || memberAccess.Name.ToString() != "LogEvent")
            {
                return;
            }
            
            var symbol = context.SemanticModel.GetSymbolInfo(memberAccess).Symbol;
            var memberSymbol = symbol as IMethodSymbol;
            if (memberSymbol == null || memberSymbol.ContainingNamespace.ToDisplayString() != "CMS.EventLog")
            {
                return;
            }


            // TODO check if invoked on right object

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

            var diagnostic = Diagnostic.Create(Rule, invocationExpression.GetLocation(), eventTypeArgument.ToString());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
