using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using BugHunter.Core;
using BugHunter.Core.Analyzers;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.BaseClassesRules.Analyzers
{
    /// <summary>
    /// Checks if User Control file inherits from right class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UserControlBaseAnalyzer : BaseClassDeclarationSyntaxAnalyzer
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
                var systemWebUiControlType = compilationContext.Compilation.GetTypeByMetadataName("System.Web.UI.UserControl");
                if (systemWebUiControlType == null)
                {
                    return;
                }

                compilationContext.RegisterSyntaxTreeAction(syntaxTreeAnalysisContext =>
                {
                    var filePath = syntaxTreeAnalysisContext.Tree.FilePath;
                    if (string.IsNullOrEmpty(filePath) || !filePath.EndsWith(FilePaths.Extensions.CONTROLS))
                    {
                        return;
                    }

                    var publicPartialInstantiableClassDeclarations = GetAllClassDeclarations(syntaxTreeAnalysisContext)
                        .Where(classDeclarationSyntax
                            => classDeclarationSyntax.IsPublic()
                            && !classDeclarationSyntax.IsAbstract()
                            && classDeclarationSyntax.IsPartial());

                    foreach (var classDeclaration in publicPartialInstantiableClassDeclarations)
                    {
                        var semanticModel = compilationContext.Compilation.GetSemanticModel(syntaxTreeAnalysisContext.Tree);
                        var baseTypeTypeSymbol = GetBaseTypeSymbol(classDeclaration, semanticModel);
                        if (baseTypeTypeSymbol != null && baseTypeTypeSymbol.Equals(systemWebUiControlType))
                        {
                            var diagnostic = CreateDiagnostic(syntaxTreeAnalysisContext, classDeclaration, Rule);
                            syntaxTreeAnalysisContext.ReportDiagnostic(diagnostic);
                        }
                    }
                });
            });
        }
    }
}
