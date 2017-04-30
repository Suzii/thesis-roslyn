using System.Collections.Immutable;
using BugHunter.AnalyzersVersions.SystemIO.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.AnalyzersVersions.SystemIO
{
    /// <summary>
    /// Searches for usages of <see cref="System.IO"/> and their access to anything other than <c>Exceptions</c> or <c>Stream</c>
    /// 
    /// Version with callback on IdentifierName and using SemanticModelBrowser
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class V00_IdentiferName_EmptyCallback : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = "V00";
        private static readonly DiagnosticDescriptor Rule = AnalyzerHelper.GetRule(DIAGNOSTIC_ID);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(c => Analyze(c, Rule), SyntaxKind.IdentifierName);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, DiagnosticDescriptor rule)
        {
            var identifierNameSyntax = context.Node as IdentifierNameSyntax;
            if (identifierNameSyntax == null)
            {
                return;
            }
            
            // just to make sure compiler does note ignore out semantic model access.. but no diagnostic should ever be raised here
            if (identifierNameSyntax.Identifier.Text == "XXX_This_Should_Never_Be_True_XXX")
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, Location.None, "XXX_This_Should_Never_Be_True_XXX"));
            }
        }
    }
}
