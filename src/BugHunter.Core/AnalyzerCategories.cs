namespace BugHunter.Core
{
    /// <summary>
    /// Enum defining the analyzer categories
    /// </summary>
    public enum AnalyzerCategories
    {
        /// <summary>
        /// Category for analyzers looking for standard .NET properties and methods with CMS equivalent that should be used instead
        /// </summary>
        CmsApiReplacements,

        /// <summary>
        /// Category for analyzers reporting inheritance of conventional .NET class over a CMS abstract class
        /// </summary>
        CmsBaseClasses,

        /// <summary>
        /// Category for analyzers reporting unintended or error-prone usage of CMS API
        /// </summary>
        InternalGuidelines,

        /// <summary>
        /// Category for analyzers reporting unintended or error-prone usage of CMS API strictly related to web presentation layer
        /// </summary>
        WebInternalGuidelines,

        /// <summary>
        /// Category for analyzers reporting usage of specific implementation over an appropriate abstraction 
        /// </summary>
        AbstractionOverImplementation,

        /// <summary>
        /// Category for analyzers reporting suspicious usage of String comparison and manipulation methods without culture specification
        /// </summary>
        StringAndCulture,

        /// <summary>
        /// Category for analyzers reporting forbidden usage of System.IO API where CMS equivalents should be used
        /// </summary>
        SystemIo,

        /// <summary>
        /// Category of analyzers solely for benchmarking purposes
        /// </summary>
        BenchmarkingBaselines,
    }
}
