using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Core.Tests.DiagnosticsFormatting
{
    internal static class AssertLocation
    {
        public static void IsWithin(Location expected, Location actual)
        {
            Assert.AreEqual(expected.SourceTree, actual.SourceTree);
            Assert.That(!actual.SourceSpan.IsEmpty);
            Assert.That(expected.SourceSpan.Start <= actual.SourceSpan.Start);
            Assert.That(expected.SourceSpan.End >= actual.SourceSpan.End);
        }

        public static void IsEmpty(Location location)
        {
            Assert.AreEqual(Location.None, location);
        }
    }
}