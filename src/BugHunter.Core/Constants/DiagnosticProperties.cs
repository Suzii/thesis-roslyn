namespace BugHunter.Core.Constants
{
    /// <summary>
    /// Helper class defining constants of key-value pairs used in Properties of <see cref="Microsoft.CodeAnalysis.Diagnostic"/>
    /// </summary>
    internal static class DiagnosticProperties
    {
        /// <summary>
        /// Constant for 'IsConditionalAccess' key
        /// </summary>
        public const string IsConditionalAccess = "IsConditionalAccess";

        /// <summary>
        /// Constant for 'IsSimpleMemberAccess' key
        /// </summary>
        public const string IsSimpleMemberAccess = "IsSimpleMemberAccess";

        /// <summary>
        /// Constant for true value
        /// </summary>
        public const string TrueFlag = "True";

        /// <summary>
        /// Constant for false value
        /// </summary>
        public const string FalseFlag = "False";
    }
}