namespace BugHunter.Core
{
    /// <summary>
    /// Class defining the analyzer category constants.
    /// </summary>
    public static class AnalyzerCategories
    {
        /// <summary>
        /// Category for analyzers looking for standard .NET properties and methods with CMS equivalent that should be used instead
        /// </summary>
        public const string CmsApiReplacements = "CmsApiReplacements";

        /// <summary>
        /// Category for analyzers reporting inheritance of conventional .NET class over a CMS abstract class
        /// </summary>
        public const string CmsBaseClasses = "CmsBaseClasses";

        /// <summary>
        /// Category for analyzers reporting unintended or error-prone usage of CMS API
        /// </summary>
        public const string InternalGuidelines = "InternalGuidelines";

        /// <summary>
        /// Category for analyzers reporting unintended or error-prone usage of CMS API strictly related to web presentation layer
        /// </summary>
        public const string WebInternalGuidelines = "WebInternalGuidelines";

        /// <summary>
        /// Category for analyzers reporting usage of specific implementation over an appropriate abstraction 
        /// </summary>
        /// TODO rename
        public const string AbstractionOverImplementation = "AbstractionOverImplementation";

        /// <summary>
        /// Category for analyzers reporting suspicious usage of String comparison and manipulation methods without culture specification
        /// </summary>
        public const string StringAndCulture = "StringAndCulture";

        /// <summary>
        /// Category for analyzers reporting forbidden usage of System.IO API where CMS equivalents should be used
        /// </summary>
        public const string SystemIo = "SystemIo";

        /// <summary>
        /// Category of analyzers solely for benchmarking purposes
        /// </summary>
        public const string BenchmarkingBaselines = "BenchmarkingBaselines";
    }
}
