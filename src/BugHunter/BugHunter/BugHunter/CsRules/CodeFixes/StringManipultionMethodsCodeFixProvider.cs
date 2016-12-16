using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.ResourceBuilder;
using BugHunter.CsRules.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.CsRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StringManipultionMethodsCodeFixProvider)), Shared]
    public class StringManipultionMethodsCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(StringManipulationMethodsAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var editor = new CodeFixHelper(context);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var root = await context.Document.GetSyntaxRootAsync();
            var invocation = root.FindNode(diagnosticSpan, true).FirstAncestorOrSelf<InvocationExpressionSyntax>(); 
;

            if (invocation == null)
            {
                return;
            }

            var methodName = invocation.Expression.ToString();

            var newMemberAccess1 = SyntaxFactory.ParseExpression($"{methodName}Invariant()");
            var newMemberAccess2 = SyntaxFactory.ParseExpression($"{methodName}(CultureInfo.CurrentCulture)");
            
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessageBuilder.GetMessage(newMemberAccess1),
                    createChangedDocument: c => editor.ReplaceExpressionWith(invocation, newMemberAccess1),
                    equivalenceKey: $"{nameof(StringManipultionMethodsCodeFixProvider)}-InvariantCulture"),
                diagnostic);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessageBuilder.GetMessage(newMemberAccess2),
                    createChangedDocument: c => editor.ReplaceExpressionWith(invocation, newMemberAccess2),
                    equivalenceKey: $"{nameof(StringManipultionMethodsCodeFixProvider)}-CurrentCulture"),
                diagnostic);
        }
    }
}