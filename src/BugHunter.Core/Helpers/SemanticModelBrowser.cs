using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core.Helpers
{
    /// <summary>
    /// Helper class for analyzing semantics of the tree
    /// </summary>
    public class SemanticModelBrowser
    {
        private readonly SemanticModel _semanticModel;

        public SemanticModelBrowser(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
        }

        public SemanticModelBrowser(SyntaxNodeAnalysisContext context)
        {
            _semanticModel = context.SemanticModel;
        }

        public SemanticModelBrowser(Compilation compilation, SyntaxTree syntaxTree)
        {
            _semanticModel = compilation.GetSemanticModel(syntaxTree);
        }

        /// <summary>
        /// Returns INamedTypeSymbol of <param name="identifierNameSyntax"></param>
        /// </summary>
        /// <param name="identifierNameSyntax">Identifier name syntax whose type symbol is to be returned</param>
        /// <returns>INamedTypeSymbol of the identifier name passed, null if no type symbol found</returns>
        public INamedTypeSymbol GetNamedTypeSymbol(IdentifierNameSyntax identifierNameSyntax)
        {
            //return _semanticModel.GetTypeInfo(identifierNameSyntax).Type as INamedTypeSymbol;

            if (identifierNameSyntax == null)
            {
                throw new ArgumentNullException(nameof(identifierNameSyntax));
            }

            var identifierNameTypeSymbol = _semanticModel.GetTypeInfo(identifierNameSyntax).Type as INamedTypeSymbol;
            if (identifierNameTypeSymbol != null)
            {
                return identifierNameTypeSymbol;
            }

            // if IdentifierName is represents a type but was not recognized previously, it can be inside of ObjectCreationExpression
            var objectCreationExpressionSyntax = identifierNameSyntax.FirstAncestorOrSelf<ObjectCreationExpressionSyntax>();
            if (objectCreationExpressionSyntax == null)
            {
                // IdentifierName was most likely part of QualifiedName and does not really represent any Type
                return null;
            }

            if (objectCreationExpressionSyntax.ArgumentList.DescendantNodes().Contains(identifierNameSyntax))
            {
                // IdentifierName is used in argument list and should have been recognized previously if it is relevent
                return null;
            }

            var qualifiedName = identifierNameSyntax.FirstAncestorOrSelf<QualifiedNameSyntax>();
            if (qualifiedName != null && qualifiedName.Parent != objectCreationExpressionSyntax)
            {
                // if identifier name is part of qualified name and not is not the last part of objectCreationExpression it does not represent a Type
                return null;
            }

            // IdentifierName is used for Object creation
            var namedSymbol = _semanticModel.GetSymbolInfo(objectCreationExpressionSyntax.Type).Symbol as INamedTypeSymbol;
            return namedSymbol;

        }
    }
}
