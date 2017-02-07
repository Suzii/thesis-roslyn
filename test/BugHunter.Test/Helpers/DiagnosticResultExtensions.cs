using System;
using Microsoft.CodeAnalysis;

namespace BugHunter.Test.Verifiers
{
    internal static class DiagnosticResultExtensions
    {
        public static DiagnosticResult WithId(this DiagnosticResult diagnosticResult, string id)
        {
            diagnosticResult.Id = id;
            return diagnosticResult;
        }

        public static DiagnosticResult WithSeverity(this DiagnosticResult diagnosticResult, DiagnosticSeverity diagnosticSeverity)
        {
            diagnosticResult.Severity = diagnosticSeverity;
            return diagnosticResult;
        }

        public static DiagnosticResult WithMessage(this DiagnosticResult diagnosticResult, string message)
        {
            diagnosticResult.Message = message;
            return diagnosticResult;
        }

        public static DiagnosticResult WithLocation(this DiagnosticResult diagnosticResult, int line, int column, FakeFileInfo fakeFileInfo, int sourceFileIndex = 0)
        {
            return WithLocation(diagnosticResult, line, column, fakeFileInfo.GetFullFilePath(sourceFileIndex));
        }
        public static DiagnosticResult WithLocation(this DiagnosticResult diagnosticResult, int line, int column, string path = "Test0.cs")
        {
            if (line < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(line), @"line must be >= -1");
            }

            if (column < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(column), @"column must be >= -1");
            }

            diagnosticResult.Locations = new[] { new DiagnosticResultLocation(path, line, column) };

            return diagnosticResult;
        }
    }
}