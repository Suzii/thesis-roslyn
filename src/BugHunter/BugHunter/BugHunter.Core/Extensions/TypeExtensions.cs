using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

// TODO delete and use directly
namespace BugHunter.Core.Extensions 
{
    public static class TypeExtensions
    {
        public static INamedTypeSymbol GetITypeSymbol(string type, SyntaxNodeAnalysisContext context)
        {
            var compilation = context.SemanticModel.Compilation;
            return GetITypeSymbol(type, compilation);
        }

        public static INamedTypeSymbol GetITypeSymbol(this Type type, SyntaxNodeAnalysisContext context)
        {
            var compilation = context.SemanticModel.Compilation;
            return GetITypeSymbol(type, compilation);
        }

        public static INamedTypeSymbol GetITypeSymbol(string type, Compilation compilation)
        {
            return compilation.GetTypeByMetadataName(type);
        }

        public static INamedTypeSymbol GetITypeSymbol(this Type type, Compilation compilation)
        {
            var typeFullName = type.FullName;
            return compilation.GetTypeByMetadataName(typeFullName);
        }
    }
}
