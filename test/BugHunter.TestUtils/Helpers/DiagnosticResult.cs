// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace BugHunter.TestUtils.Helpers
{
    /// <summary>
    /// Struct that stores information about a Diagnostic appearing in a source
    /// </summary>
    public struct DiagnosticResult
    {
        private DiagnosticResultLocation[] _locations;

        public DiagnosticResultLocation[] Locations
        {
            get
            {
                return _locations ?? (_locations = new DiagnosticResultLocation[] { });
            }

            set
            {
                _locations = value;
            }
        }

        public DiagnosticSeverity Severity
        {
            get; set;
        }

        public string Id
        {
            get; set;
        }

        public string Message
        {
            get; set;
        }

        public string Path => Locations.Length > 0 ? Locations[0].Path : string.Empty;

        public int Line => Locations.Length > 0 ? Locations[0].Line : -1;

        public int Column => Locations.Length > 0 ? Locations[0].Column : -1;
    }
}