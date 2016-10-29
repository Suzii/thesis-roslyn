using System;
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
        /// Returns ITypeSymbol of target whose member is being accessed.
        /// </summary>
        /// <param name="memberAccess">Member access to be analyzed for target type</param>
        /// <returns>ITypeSymbol of the object that member belongs to</returns>
        public ITypeSymbol GetMemberAccessTarget(MemberAccessExpressionSyntax memberAccess)
        {
            if (memberAccess == null)
            {
                throw new ArgumentNullException(nameof(memberAccess));
            }

            if (memberAccess.Expression is LiteralExpressionSyntax || memberAccess.Expression is IdentifierNameSyntax)
            {
                return _semanticModel.GetTypeInfo(memberAccess.Expression).Type;
            }

            if (memberAccess.Expression is MemberAccessExpressionSyntax)
            {
                var nestedMemberAccess = memberAccess.Expression as MemberAccessExpressionSyntax;
                return _semanticModel.GetTypeInfo(nestedMemberAccess.Name).Type;
            }
            
            if (memberAccess.Expression is MemberBindingExpressionSyntax)
            {
                var bindingMemberAccess = memberAccess.Expression as MemberBindingExpressionSyntax;
                return _semanticModel.GetTypeInfo(bindingMemberAccess).Type;
            }

            if (memberAccess.Expression is ThisExpressionSyntax || memberAccess.Expression is BaseExpressionSyntax)
            {
                var instance = (InstanceExpressionSyntax) memberAccess.Expression;
                return _semanticModel.GetTypeInfo(instance).Type;
            }

            if (memberAccess.Expression is InvocationExpressionSyntax)
            {
                var invocation = memberAccess.Expression as InvocationExpressionSyntax;
                var methodSymbol = _semanticModel.GetSymbolInfo(invocation.Expression).Symbol as IMethodSymbol;
                return methodSymbol?.ReturnType;
            }

            if (memberAccess.Expression is ObjectCreationExpressionSyntax)
            {
                var objectCreationExpressionSyntax = memberAccess.Expression as ObjectCreationExpressionSyntax;
                var namedSymbol = _semanticModel.GetSymbolInfo(objectCreationExpressionSyntax.Type).Symbol as INamedTypeSymbol;

                return namedSymbol;
            }

            // TODO return null or throw an exception?
            throw new ArgumentException(@"Could not detect target for this member access.", nameof(memberAccess));
        }
    }
}
