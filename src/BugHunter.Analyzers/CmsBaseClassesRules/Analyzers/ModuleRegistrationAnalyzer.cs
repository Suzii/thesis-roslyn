using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core;
using BugHunter.Core.Analyzers;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsBaseClassesRules.Analyzers
{
    /// <summary>
    /// Checks Modules and ModuleEntries are registered in file where they are declared
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ModuleRegistrationAnalyzer : BaseClassDeclarationSyntaxAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.MODULE_REGISTRATION;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
                title: new LocalizableResourceString(nameof(CmsBaseClassesResources.ModuleRegistration_Title), CmsBaseClassesResources.ResourceManager, typeof(CmsBaseClassesResources)),
                messageFormat: new LocalizableResourceString(nameof(CmsBaseClassesResources.ModuleRegistration_MessageFormat), CmsBaseClassesResources.ResourceManager, typeof(CmsBaseClassesResources)),
                category: nameof(AnalyzerCategories.CmsBaseClasses),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(CmsBaseClassesResources.ModuleRegistration_Description), CmsBaseClassesResources.ResourceManager, typeof(CmsBaseClassesResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            
            context.RegisterCompilationStartAction(compilationContext =>
            {
                var moduleEntryType = compilationContext.Compilation.GetTypeByMetadataName("CMS.Core.ModuleEntry");
                var moduleType = compilationContext.Compilation.GetTypeByMetadataName("CMS.DataEngine.Module");
                if (moduleEntryType == null || moduleType == null)
                {
                    return;
                }

                // TODO try different order of expressions to tweak with performance and do some benchmarks
                compilationContext.RegisterSyntaxTreeAction(syntaxTreeAnalysisContext =>
                {
                    var filePath = syntaxTreeAnalysisContext.Tree.FilePath;
                    if (string.IsNullOrEmpty(filePath))
                    {
                        return;
                    }

                    var publicInstantiableClassDeclarations = GetAllClassDeclarations(syntaxTreeAnalysisContext)
                        .Where(classDeclarationSyntax
                            => classDeclarationSyntax.IsPublic()
                            && !classDeclarationSyntax.IsAbstract())
                        .ToArray();

                    if (!publicInstantiableClassDeclarations.Any())
                    {
                        return;
                    }

                    var semanticModel = compilationContext.Compilation.GetSemanticModel(syntaxTreeAnalysisContext.Tree);
                    var registeredModuleTypeSyntaxes = GetRegisteredModuleTypeSyntaxes(syntaxTreeAnalysisContext);
                    var registeredModuleTypes = registeredModuleTypeSyntaxes.Select(typeSyntax => semanticModel.GetSymbolInfo(typeSyntax).Symbol);

                    foreach (var classDeclaration in publicInstantiableClassDeclarations)
                    {
                        var classTypeSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);
                        // TODO maybe move the first check up?
                        if (IsModuleOrModuleEntry(classTypeSymbol, moduleEntryType, moduleType) && !IsModuleRegistered(registeredModuleTypes, classTypeSymbol))
                        {
                            var diagnostic = CreateDiagnostic(syntaxTreeAnalysisContext, classDeclaration, Rule);
                            syntaxTreeAnalysisContext.ReportDiagnostic(diagnostic);
                        }
                    }
                });
            });
        }

        private static bool IsModuleOrModuleEntry(INamedTypeSymbol classTypeSymbol, INamedTypeSymbol moduleEntryType, INamedTypeSymbol moduleType)
        {
            return classTypeSymbol != null && (classTypeSymbol.IsDerivedFromClassOrInterface(moduleEntryType) || classTypeSymbol.IsDerivedFromClassOrInterface(moduleType));
        }

        private static bool IsModuleRegistered(IEnumerable<ISymbol> registeredModuleTypes, INamedTypeSymbol moduleToBeChecked)
        {
            var registeredModules = registeredModuleTypes as ISymbol[] ?? registeredModuleTypes.ToArray();
            return registeredModules.Any(registeredModule => registeredModule.Equals(moduleToBeChecked));
        }

        private static IEnumerable<TypeSyntax> GetRegisteredModuleTypeSyntaxes(SyntaxTreeAnalysisContext syntaxTreeAnalysisContext)
        {
            var allAttributeListSyntaxes = syntaxTreeAnalysisContext
                .Tree
                .GetRoot()
                .DescendantNodesAndSelf()
                .OfType<AttributeListSyntax>();

            var assemblyAttributeListSyntaxes = allAttributeListSyntaxes
                .Where(attributeList => attributeList.Target.Identifier.IsKind(SyntaxKind.AssemblyKeyword));

            var registerModuleAssemblyAttributes = assemblyAttributeListSyntaxes
                .SelectMany(attributeList => attributeList.Attributes)
                .Where(attribute => attribute.Name.ToString() == "RegisterModule");

            var registeredModuleTypeSyntaxes = registerModuleAssemblyAttributes
                .SelectMany(attribute => attribute.ArgumentList.Arguments)
                .Select(argument => argument.Expression)
                .Where(argumentExpression => argumentExpression.IsKind(SyntaxKind.TypeOfExpression))
                .Cast<TypeOfExpressionSyntax>()
                .Select(typeOfExpression => typeOfExpression.Type);

            return registeredModuleTypeSyntaxes;
        }
    }
}
