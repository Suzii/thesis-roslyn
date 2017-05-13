using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace BugHunter.Core.Extensions
{
    /// <summary>
    /// Helper class containing extension methods for <see cref="INamedTypeSymbol"/> and <see cref="ITypeSymbol"/>
    /// Inspired by http://fossies.org/linux/misc/mono-sources/monodevelop/monodevelop-6.0.0.4761.tar.gz/monodevelop-6.0/src/addins/CSharpBinding/Util/TypeExtensions.cs?m=t
    /// </summary>
    public static class TypeSymbolExtensions
    {
        /// <summary>
        /// Determines whether <param name="namedTypeSymbol"></param> is a nested in some other named type
        /// </summary>
        /// <param name="namedTypeSymbol">TypeSymbol to be examined</param>
        /// <returns>True if <param name="namedTypeSymbol"></param>is nested</returns>
        public static bool IsNested(this INamedTypeSymbol namedTypeSymbol)
            => namedTypeSymbol.ContainingSymbol.Kind == SymbolKind.NamedType;

        /// <summary>
        /// Determines whether <param name="namedTypeSymbol"></param> has only <see cref="object"/> in inheritance hierarchy
        /// </summary>
        /// <param name="namedTypeSymbol">TypeSymbol to be examined</param>
        /// <returns>True if type symbol only extends <see cref="object"/></returns>
        public static bool ExtendsOnlyObject(this INamedTypeSymbol namedTypeSymbol)
            => namedTypeSymbol?.BaseType?.SpecialType == SpecialType.System_Object;

        /// <summary>
        /// Gets the invoke method for a delegate type.
        /// </summary>
        /// <remarks>
        /// Returns null if the type is not a delegate type; or if the invoke method could not be found.
        /// </remarks>
        public static IMethodSymbol GetDelegateInvokeMethod(this ITypeSymbol type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.TypeKind != TypeKind.Delegate)
            {
                return null;
            }

            return type.GetMembers("Invoke")
                        .OfType<IMethodSymbol>()
                        .FirstOrDefault(m => m.MethodKind == MethodKind.DelegateInvoke);
        }

        /// <summary>
        /// Determines whether <param name="type"></param> is nullable
        /// </summary>
        /// <param name="type">Type to be examined</param>
        /// <returns>True if type is nullable</returns>
        public static bool IsNullableType(this ITypeSymbol type)
            => type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;

        /// <summary>
        /// Returns underlying type is nullable type, null otherwise.
        /// </summary>
        /// <param name="type">Nullable type to be examined for underlying type</param>
        /// <returns>Underlying type of the passed nullable type, null if type was not nullable</returns>
        public static ITypeSymbol GetNullableUnderlyingType(this ITypeSymbol type)
        {
            if (!IsNullableType(type))
            {
                return null;
            }

            return ((INamedTypeSymbol)type).TypeArguments[0];
        }

        /// <summary>
        /// Gets all base classes and interfaces.
        /// </summary>
        /// <returns>All classes and interfaces.</returns>
        /// <param name="type">Type to be examined</param>
        /// <param name="includeSelf">Should accessed type itself be included, default is false</param>
        public static IEnumerable<INamedTypeSymbol> GetAllBaseClassesAndInterfaces(this INamedTypeSymbol type, bool includeSelf = false)
        {
            if (!includeSelf)
            {
                type = type.BaseType;
            }

            var currentType = type;
            while (currentType != null)
            {
                yield return currentType;
                currentType = currentType.BaseType;
            }

            foreach (var inter in type.AllInterfaces)
            {
                yield return inter;
            }
        }

        /// <summary>
        /// Determines if derived from baseType. Includes itself, all base classes and all interfaces.
        /// Consider using the second overload as the performance should be better.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="baseType">Base type.</param>
        /// <returns><c>true</c> if is derived from the specified type baseType; otherwise, <c>false</c>.</returns>
        public static bool IsDerivedFrom(this INamedTypeSymbol type, INamedTypeSymbol baseType)
        {
            if (baseType == null)
            {
                return false;
            }

            baseType = baseType.SafelyConstructUnboundGenericType();

            var currentType = type;
            while (currentType != null)
            {
                currentType = currentType.SafelyConstructUnboundGenericType();

                if (Equals(currentType, baseType))
                {
                    return true;
                }

                currentType = currentType.BaseType;
            }

            var interfaces = type.AllInterfaces.Select(i => i.SafelyConstructUnboundGenericType());

            return interfaces.Contains(baseType);
        }

        /// <summary>
        /// Determines if derived from baseType. Includes itself, all base classes and all interfaces.
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <param name="baseTypeOrInterfaceName">Fully qualified name of the class or interface the <paramref name="type"/> should be derived from</param>
        /// <param name="compilation">Containing compilation</param>
        /// <returns><c>true</c> if is derived from the specified type baseType; otherwise, <c>false</c>.</returns>
        public static bool IsDerivedFrom(this INamedTypeSymbol type, string baseTypeOrInterfaceName, Compilation compilation)
        {
            if (string.Equals(type.ConstructedFrom.ToString(), baseTypeOrInterfaceName, StringComparison.Ordinal))
            {
                return true;
            }

            var baseClassOrInterfaceType = compilation.GetTypeByMetadataName(baseTypeOrInterfaceName);

            return baseClassOrInterfaceType != null && type.IsDerivedFrom(baseClassOrInterfaceType);
        }

        /// <summary>
        /// Returns an unbound generic type of this named type if it is generic, or self otherwise
        /// </summary>
        /// <param name="type">Type to be sagely unbounded</param>
        /// <returns>Type with unbounded generics or type itself if it is not generic</returns>
        private static INamedTypeSymbol SafelyConstructUnboundGenericType(this INamedTypeSymbol type)
        {
            return type.IsGenericType ? type.ConstructUnboundGenericType() : type;
        }
    }
}
