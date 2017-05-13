using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    /// <summary>
    /// Diagnostic formatter for <see cref="MemberAccessExpressionSyntax"/> nodes, where only name part should be reflected in raised diagnostic
    /// </summary>
    internal class MemberAccessOnlyDiagnosticFormatter : DefaultDiagnosticFormatter<MemberAccessExpressionSyntax>
    {
        /// <summary>
        /// Creates a <see cref="Diagnostic"/> from <paramref name="descriptor" /> based on passed <paramref name="memberAccess" />.
        ///
        /// MessageFormat will be passed an argument with string representation of 'Name' part of passed <paramref name="memberAccess" />.
        /// Location will be only of 'Name' part of passed <paramref name="memberAccess" />.
        /// </summary>
        /// <param name="descriptor">Diagnostic descriptor for diagnostic to be created</param>
        /// <param name="memberAccess">Member access that the diagnostic should be raised for</param>
        /// <returns>Diagnostic created from descriptor for given member access</returns>
        public override Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, MemberAccessExpressionSyntax memberAccess)
            => base.CreateDiagnostic(descriptor, memberAccess).MarkAsSimpleMemberAccess();

        /// <summary>
        /// Returns location of Name property of passed <paramref name="memberAccess" />
        /// </summary>
        /// <param name="memberAccess">Member access whose location of Name property should be returned</param>
        /// <returns>Location of Name property of passed member access; Empty location if anything is null</returns>
        protected override Location GetLocation(MemberAccessExpressionSyntax memberAccess)
            => memberAccess?.Name?.GetLocation();

        /// <summary>
        /// Returns string representation of Name property of passed <paramref name="memberAccess" />
        /// </summary>
        /// <param name="memberAccess">Member access whose string representation of Name property should be returned</param>
        /// <returns>String representation of Name property of passed member access; Empty string if anything is null</returns>
        protected override string GetDiagnosedUsage(MemberAccessExpressionSyntax memberAccess)
            => memberAccess?.Name?.Identifier.ValueText;
    }
}