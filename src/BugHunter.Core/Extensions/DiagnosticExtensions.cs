// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using BugHunter.Core.Constants;
using Microsoft.CodeAnalysis;

namespace BugHunter.Core.Extensions
{
     /// <summary>
     /// Helper class containing extensions for <see cref="Diagnostic"/>
     /// </summary>
    public static class DiagnosticExtensions
    {
        /// <summary>
        /// Marks the diagnostic as being raised on conditional access by adding a flag to its <see cref="DiagnosticProperties"/>
        /// </summary>
        /// <param name="diagnostic">Diagnostic to be marked</param>
        /// <returns>Diagnostic with added flag</returns>
        public static Diagnostic MarkAsConditionalAccess(this Diagnostic diagnostic)
        {
            var newProperties = diagnostic.Properties.SetItem(DiagnosticProperties.IsConditionalAccess, DiagnosticProperties.TrueFlag);

            return diagnostic.WithNewProperties(newProperties);
        }

        /// <summary>
        /// Determines whether the diagnostic was raised on conditional access by inspecting flags in its <see cref="DiagnosticProperties"/>
        /// </summary>
        /// <param name="diagnostic">Diagnostic to be inspected</param>
        /// <returns>True if diagnostic contains mark for simple member access</returns>
        public static bool IsMarkedAsConditionalAccess(this Diagnostic diagnostic)
        {
            string conditionalAccessValue;
            return diagnostic.Properties.TryGetValue(DiagnosticProperties.IsConditionalAccess, out conditionalAccessValue)
                && conditionalAccessValue == DiagnosticProperties.TrueFlag;
        }

        /// <summary>
        /// Marks the diagnostic as being raised on simple member access by adding a flag to its <see cref="DiagnosticProperties"/>
        /// </summary>
        /// <param name="diagnostic">Diagnostic to be marked</param>
        /// <returns>Diagnostic with added flag</returns>
        public static Diagnostic MarkAsSimpleMemberAccess(this Diagnostic diagnostic)
        {
            var newProperties = diagnostic.Properties.SetItem(DiagnosticProperties.IsSimpleMemberAccess, DiagnosticProperties.TrueFlag);

            return diagnostic.WithNewProperties(newProperties);
        }

        /// <summary>
        /// Determines whether the diagnostic was raised on simple member access by inspecting flags in its <see cref="DiagnosticProperties"/>
        /// </summary>
        /// <param name="diagnostic">Diagnostic to be inspected</param>
        /// <returns>True if diagnostic contains mark for simple member access</returns>
        public static bool IsMarkedAsSimpleMemberAccess(this Diagnostic diagnostic)
        {
            string simpleMemberAccessValue;
            return diagnostic.Properties.TryGetValue(DiagnosticProperties.IsSimpleMemberAccess, out simpleMemberAccessValue)
                && simpleMemberAccessValue == DiagnosticProperties.TrueFlag;
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