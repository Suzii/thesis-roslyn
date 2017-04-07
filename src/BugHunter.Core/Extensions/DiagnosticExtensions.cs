using System.Collections.Immutable;
using BugHunter.Core.Constants;
using Microsoft.CodeAnalysis;

namespace BugHunter.Core.Extensions
{
    public static class DiagnosticExtensions
    {
        public static Diagnostic MarkAsConditionalAccess(this Diagnostic diagnostic)
        {
            var newProperties = diagnostic.Properties.SetItem(DiagnosticProperties.IS_CONDITIONAL_ACCESS, DiagnosticProperties.TRUE_FLAG);

            return diagnostic.WithNewProperties(newProperties);
        }

        public static bool IsMarkedAsConditionalAccess(this Diagnostic diagnostic)
        {
            string conditionalAccessValue;
            return diagnostic.Properties.TryGetValue(DiagnosticProperties.IS_CONDITIONAL_ACCESS, out conditionalAccessValue) 
                && conditionalAccessValue == DiagnosticProperties.TRUE_FLAG;
        }

        public static Diagnostic MarkAsSimpleMemberAccess(this Diagnostic diagnostic)
        {
            var newProperties = diagnostic.Properties.SetItem(DiagnosticProperties.IS_SIMPLE_MEMBER_ACCESS, DiagnosticProperties.TRUE_FLAG);

            return diagnostic.WithNewProperties(newProperties);
        }

        public static bool IsMarkedAsSimpleMemberAccess(this Diagnostic diagnostic)
        {
            string simpleMemberAccessValue;
            return diagnostic.Properties.TryGetValue(DiagnosticProperties.IS_SIMPLE_MEMBER_ACCESS, out simpleMemberAccessValue)
                && simpleMemberAccessValue == DiagnosticProperties.TRUE_FLAG;
        }

        private static Diagnostic WithNewProperties(this Diagnostic diagnostic, ImmutableDictionary<string, string> newProperties)
        {
            var newDiagnostic = Diagnostic.Create(
                diagnostic.Id,
                diagnostic.Descriptor.Category,
                diagnostic.GetMessage(),
                diagnostic.Severity,
                diagnostic.DefaultSeverity,
                diagnostic.Descriptor.IsEnabledByDefault,
                diagnostic.WarningLevel,
                diagnostic.Descriptor.Title,
                diagnostic.Descriptor.Description,
                diagnostic.Descriptor.HelpLinkUri,
                diagnostic.Location,
                diagnostic.AdditionalLocations,
                diagnostic.Descriptor.CustomTags,
                newProperties);

            return newDiagnostic;
        }
    }
}