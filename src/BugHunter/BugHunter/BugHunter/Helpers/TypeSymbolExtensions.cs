using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace BugHunter.Helpers
{
    /// <summary>
    /// Inspired by http://fossies.org/linux/misc/mono-sources/monodevelop/monodevelop-6.0.0.4761.tar.gz/monodevelop-6.0/src/addins/CSharpBinding/Util/TypeExtensions.cs?m=t
    /// </summary>
    public static class TypeSymbolExtensions
    {
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
        
        public static bool IsNullableType(this ITypeSymbol type)
        {
            var original = type.OriginalDefinition;
            return original.SpecialType == SpecialType.System_Nullable_T;
        }

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
        /// <param name="includeSuperType">Should supertype be included, default is false</param>
        public static IEnumerable<INamedTypeSymbol> GetAllBaseClassesAndInterfaces(this INamedTypeSymbol type, bool includeSuperType = false)
        {
            if (!includeSuperType)
            {
                type = type.BaseType;
            }

            var curType = type;
            while (curType != null)
            {
                yield return curType;
                curType = curType.BaseType;
            }

            foreach (var inter in type.AllInterfaces)
            {
                yield return inter;
            }
        }

        /// <summary>
        /// Determines if derived from baseType. Includes itself, all base classes and all interfaces.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="baseType">Base type.</param>
        /// <param name="shouldUnboundGenerics">If true, any base types or implemented interfaces will be compared with unbounded generics</param>
        /// <returns><c>true</c> if is derived from the specified type baseType; otherwise, <c>false</c>.</returns>
        public static bool IsDerivedFromClassOrInterface(this INamedTypeSymbol type, INamedTypeSymbol baseType, bool shouldUnboundGenerics = false)
        {
            if (shouldUnboundGenerics)
            {
                baseType = baseType.SafelyConstructUnboundGenericType();
            }

            var curType = type;
            while (curType != null)
            {
                if (shouldUnboundGenerics)
                {
                    curType = curType.SafelyConstructUnboundGenericType();
                }

                if (Equals(curType, baseType))
                {
                    return true;
                }

                curType = curType.BaseType;
            }

            var interfaces = shouldUnboundGenerics
                ? type.AllInterfaces.Select(i => i.SafelyConstructUnboundGenericType())
                : type.AllInterfaces;

            return interfaces.Contains(baseType);
        }

        /// <summary>
        /// Returns an unbound generic type of this named type if it is generic, or self otherwise
        /// </summary>
        /// <param name="type">Type to be sagely unbounded</param>
        /// <returns>Type with unbounded generics or type itself if it is not generic</returns>
        public static INamedTypeSymbol SafelyConstructUnboundGenericType(this INamedTypeSymbol type)
        {
            return type.IsGenericType ? type.ConstructUnboundGenericType() : type;
        }
    }
}
