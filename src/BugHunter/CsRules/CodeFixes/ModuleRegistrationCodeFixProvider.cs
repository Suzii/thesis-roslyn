using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.CsRules.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.CsRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ModuleRegistrationCodeFixProvider)), Shared]
    public class ModuleRegistrationCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(ModuleRegistrationAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var classDeclarationCodeFixHelper = new ClassDeclarationCodeFixHelper(context);

            var diagnostic = classDeclarationCodeFixHelper.GetFirstDiagnostic(FixableDiagnosticIds.ToArray());
            var diagnosticId = diagnostic.Id;
            var classDeclaration = await classDeclarationCodeFixHelper.GetDiagnosedClassDeclarationSyntax(diagnosticId);
            if (classDeclaration == null)
            {
                return;
            }

            var className = classDeclaration.Identifier.ToString();

            var containingNamespaceNode = classDeclaration
                .AncestorsAndSelf()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault();

            var containingNamespaceName = containingNamespaceNode?.Name.ToString();

            var attributeName = SyntaxFactory.ParseName("RegisterModule");
            var attributeArgumentListSyntax = SyntaxFactory.ParseAttributeArgumentList($"(typeof({className}))");
            var separatedSyntaxList = new SeparatedSyntaxList<AttributeSyntax>();
            separatedSyntaxList = separatedSyntaxList.Add(SyntaxFactory.Attribute(attributeName, attributeArgumentListSyntax));
            var attributeTargetSpecifierSyntax = SyntaxFactory.AttributeTargetSpecifier(SyntaxFactory.Identifier("assembly"));
            var attributeToBeAdded = SyntaxFactory.AttributeList(attributeTargetSpecifierSyntax, separatedSyntaxList);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: $"Add attribute: {className}.",
                    createChangedDocument: c => classDeclarationCodeFixHelper.ApplyRootModification(oldRoot => oldRoot.InsertNodesBefore(containingNamespaceNode, new[] {attributeToBeAdded}), "CMS", containingNamespaceName),
                    equivalenceKey: nameof(ModuleRegistrationCodeFixProvider)),
                diagnostic);
        }
    }
}