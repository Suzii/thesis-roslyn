using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core.Analyzers;
using BugHunter.Core.Constants;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.DiagnosticDescriptionBuilders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Web.Analyzers.CmsBaseClassesRules.Analyzers
{
    /// <summary>
    /// Checks if User Control file inherits from right class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UserControlBaseAnalyzer : BaseClassDeclarationSyntaxAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.USER_CONTROL_BASE;

        private static readonly DiagnosticDescriptor Rule = BaseClassesInheritanceRuleBuilder.GetRule(DIAGNOSTIC_ID, "User Control", "some abstract CMSUserControl");

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
                    if (string.IsNullOrEmpty(filePath) || !filePath.EndsWith(FileExtensions.CONTROLS))
                    {
                        return;
                    }

                    var publicPartialInstantiableClassDeclarations = GetAllClassDeclarations(syntaxTreeAnalysisContext)
                        .Where(classDeclarationSyntax
                            => classDeclarationSyntax.IsPublic()
                            && !classDeclarationSyntax.IsAbstract()
                            && classDeclarationSyntax.IsPartial())
                        .ToArray();

                    if (!publicPartialInstantiableClassDeclarations.Any())
                    {
                        return;
                    }


                    var semanticModel = compilationContext.Compilation.GetSemanticModel(syntaxTreeAnalysisContext.Tree);

                    foreach (var classDeclaration in publicPartialInstantiableClassDeclarations)
                    {
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
