using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.BaseClassesRules.Analyzers
{
    /// <summary>
    /// Checks if Web Part file inherits from right class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UserControlBaseAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.USER_CONTROL_BASE;

        // TODO think of nicer messages
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
                title: "User control must inherit the right class",
                messageFormat: "'{0}' should inherit from some abstract CMSControl.",
                category: AnalyzerCategories.CS_RULES,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: "User control must inherit the right class.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(compilationContext =>
            {
                var systemWebUiPageType = compilationContext.Compilation.GetTypeByMetadataName("System.Web.UI.Control");
                if (systemWebUiPageType == null)
                {
                    return;
                }

                compilationContext.RegisterSyntaxTreeAction(syntaxTreeAnalysisContext =>
                {
                    var filePath = syntaxTreeAnalysisContext.Tree.FilePath;
                    if (string.IsNullOrEmpty(filePath) || !(filePath.Contains(ProjectPaths.USER_CONTROLS)))
                    {
                        return;
                    }

                    var root = syntaxTreeAnalysisContext.Tree.GetRoot();
                    var publicPartialInstantiableClassDeclarations = root
                        .DescendantNodesAndSelf()
                        .OfType<ClassDeclarationSyntax>()
                        .Where(classDeclarationSyntax
                            => IsPublicClass(classDeclarationSyntax)
                            && !IsAbstractClass(classDeclarationSyntax)
                            && IsPartialClass(classDeclarationSyntax));

                    foreach (var classDeclaration in publicPartialInstantiableClassDeclarations)
                    {
                        if (classDeclaration.BaseList == null || classDeclaration.BaseList.Types.IsNullOrEmpty())
                        {
                            return;
                        }

                        var semanticModel = compilationContext.Compilation.GetSemanticModel(syntaxTreeAnalysisContext.Tree);
                        var baseTypeTypeSymbol = semanticModel.GetDeclaredSymbol(classDeclaration).BaseType;
                        if (baseTypeTypeSymbol == null)
                        {
                            return;
                        }

                        if (baseTypeTypeSymbol.Equals(systemWebUiPageType))
                        {
                            var location = syntaxTreeAnalysisContext.Tree.GetLocation(classDeclaration.Identifier.FullSpan);
                            var diagnostic = Diagnostic.Create(Rule, location, classDeclaration.Identifier.ToString());
                            syntaxTreeAnalysisContext.ReportDiagnostic(diagnostic);
                        }
                    }
                });
            });
        }

        private bool IsPartialClass(ClassDeclarationSyntax classDeclarationSyntax)
        {
            return classDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword));
        }

        private static bool IsAbstractClass(ClassDeclarationSyntax classDeclarationSyntax)
        {
            return classDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.AbstractKeyword));
        }

        private static bool IsPublicClass(ClassDeclarationSyntax classDeclarationSyntax)
        {
            return classDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword));
        }
    }
}
