using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Helpers
{
    public static class TypesHelper
    {
        public static INamedTypeSymbol GetITypeSymbol(Type type, SyntaxNodeAnalysisContext context)
        {
            var compilation = context.SemanticModel.Compilation;
            return GetITypeSymbol(type, compilation);
        }
        public static INamedTypeSymbol GetITypeSymbol(Type type, Compilation compilation)
        {
            var typeFullName = type.FullName;
            return compilation.GetTypeByMetadataName(typeFullName);
        }
    }
}
