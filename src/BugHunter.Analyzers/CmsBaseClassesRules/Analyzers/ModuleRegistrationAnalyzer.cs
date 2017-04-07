using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core;
using BugHunter.Core.DiagnosticsFormatting;
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
    public class ModuleRegistrationAnalyzer : DiagnosticAnalyzer
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

        private static readonly ISymbolDiagnosticFormatter<INamedTypeSymbol> DiagnosticFormatter = DiagnosticFormatterFactory.CreateNamedTypeSymbolFormatter();
        
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            
            context.RegisterSymbolAction(symbolAnalysisContext =>
            {
                var namedTypeSymbol = symbolAnalysisContext.Symbol as INamedTypeSymbol;

                if (namedTypeSymbol == null ||
                    namedTypeSymbol.IsAbstract ||
                    (!namedTypeSymbol.IsDerivedFrom("CMS.Core.ModuleEntry", symbolAnalysisContext.Compilation) &&
                    !namedTypeSymbol.IsDerivedFrom("CMS.DataEngine.Module", symbolAnalysisContext.Compilation)))
                {
                    return;
                }

                // if it is partial definition we might get some false positive diagnostics
                var syntaxTree = namedTypeSymbol.Locations.FirstOrDefault()?.SourceTree;

                var registeredModuleTypeSyntaxes = GetRegisteredModuleTypeSyntaxes(syntaxTree);

                var moduleTypeSyntaxes = registeredModuleTypeSyntaxes as IList<TypeSyntax> ?? registeredModuleTypeSyntaxes.ToList();
                if (moduleTypeSyntaxes.Any())
                {
                    var semanticModel = symbolAnalysisContext.Compilation.GetSemanticModel(syntaxTree);
                    var registeredModuleTypes = moduleTypeSyntaxes
                        .Select(typeSyntax => semanticModel.GetSymbolInfo(typeSyntax).Symbol);

                    if (IsModuleRegistered(registeredModuleTypes, namedTypeSymbol))
                    {
                        return;
                    }
                }

                var diagnostic = DiagnosticFormatter.CreateDiagnostic(Rule, namedTypeSymbol);
                symbolAnalysisContext.ReportDiagnostic(diagnostic);

            }, SymbolKind.NamedType);
        }

        private static bool IsModuleOrModuleEntry(INamedTypeSymbol classTypeSymbol, INamedTypeSymbol moduleEntryType, INamedTypeSymbol moduleType)
        {
            return classTypeSymbol != null && (classTypeSymbol.IsDerivedFrom(moduleEntryType) || classTypeSymbol.IsDerivedFrom(moduleType));
        }

        private static bool IsModuleRegistered(IEnumerable<ISymbol> registeredModuleTypes, INamedTypeSymbol moduleToBeChecked)
        {
            var registeredModules = registeredModuleTypes as ISymbol[] ?? registeredModuleTypes.ToArray();
            return registeredModules.Any(registeredModule => registeredModule.Equals(moduleToBeChecked));
        }

        private static IEnumerable<TypeSyntax> GetRegisteredModuleTypeSyntaxes(SyntaxTree syntaxTree)
        {
            var assemblyAttributeListSyntaxes = GetAttributeListSyntaxes(syntaxTree)
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

        private static IEnumerable<AttributeListSyntax> GetAttributeListSyntaxes(SyntaxTree syntaxTree)
        {
            if (syntaxTree.HasCompilationUnitRoot)
            {
                return syntaxTree.GetCompilationUnitRoot().AttributeLists;
            }

            return syntaxTree
                .GetRoot()
                .DescendantNodesAndSelf()
                .OfType<AttributeListSyntax>();
        }
    }
}
