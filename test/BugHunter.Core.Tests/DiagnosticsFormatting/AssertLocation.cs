using BugHunter.TestUtils.Helpers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Core.Tests.DiagnosticsFormatting
{
    internal static class AssertLocation
    {
        public static void IsWithin(Location expected, Location actual)
        {
            Assert.AreEqual(expected.SourceTree, actual.SourceTree);
            Assert.That(!actual.SourceSpan.IsEmpty, "Location should not be empty");
            Assert.That(expected.SourceSpan.Start <= actual.SourceSpan.Start, $"Location start {actual.SourceSpan.Start} must not be smaller than {expected.SourceSpan.Start}");
            Assert.That(expected.SourceSpan.End >= actual.SourceSpan.End, $"Location end {actual.SourceSpan.End} must not be greater then than {expected.SourceSpan.End}");
        }

        public static void IsWithinOnOneLine(DiagnosticResultLocation expected, Location actual)
        {
            var actualSpan = actual.GetLineSpan();

            Assert.IsTrue(actualSpan.Path == expected.Path || (actualSpan.Path != null && actualSpan.Path.Contains("Test0.") && expected.Path.Contains("Test.")),
                $"Expected diagnostic to be in file \"{expected.Path}\" was actually in file \"{actualSpan.Path}\"");

            var actualLinePosition = actualSpan.StartLinePosition;

            // Only check line position if there is an actual line in the real diagnostic
            if (actualLinePosition.Line > 0)
            {
                Assert.AreEqual(expected.Line, actualLinePosition.Line + 1,
                    $"Expected diagnostic to be on line \"{expected.Line}\" was actually on line \"{actualLinePosition.Line + 1}\"");
            }
            
            // Only check column position if there is an actual column position in the real diagnostic
            if (actualLinePosition.Character > 0)
            {
                Assert.That(expected.Column <= actualLinePosition.Character + 1,
                    $"Expected diagnostic to start at column greater than \"{expected.Column - 1}\" was actually at column \"{actualLinePosition.Character + 1}\"");
            }
        }

        public static void IsEmpty(Location location)
        {
            Assert.AreEqual(Location.None, location);
        }
    }
}