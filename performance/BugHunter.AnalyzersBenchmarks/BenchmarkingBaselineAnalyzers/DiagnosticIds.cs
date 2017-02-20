namespace BugHunter.AnalyzersBenchmarks.BenchmarkingBaselineAnalyzers
{
    /// <summary>
    /// Class defining the IDs for diagnostic analyzers.
    /// </summary>
    internal static class DiagnosticIds
    {
        // This is section for artificial IDs only for benchmarking purposes
        public const string ALL_SYNTAX_NODES = "BHXXX0";
        public const string SYNTAX_NODE_IDENTIFIER_NAME = "BHXXX1";
        public const string SYNTAX_NODE_MEMBER_ACCESS_EMPTY_CALLBACK = "BHXX02";
        public const string SYNTAX_NODE_MEMBER_ACCESS_SINGLE_CALLBACK = "BHXX12";
        public const string SYNTAX_NODE_MEMBER_ACCESS_TWO_CALLBACKS = "BHXX22";
        public const string SYNTAX_NODE_INVOCATION_EXPRESSION = "BHXXX3";
        public const string SYNTAX_NODE_ELEMENT_ACCESS = "BHXXX4";
        public const string SYNTAX_NODE_CLASS_DECLARATION = "BHXXX5";
    }
}
