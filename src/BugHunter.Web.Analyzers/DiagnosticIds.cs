namespace BugHunter.Web.Analyzers
{
    /// <summary>
    /// Class defining the IDs for diagnostic analyzers.
    /// </summary>
    internal static class DiagnosticIds
    {
        // CmsBaseClasses
        public const string WEB_PART_BASE = "BH4000";
        public const string UI_WEB_PART_BASE = "BH4002";
        public const string PAGE_BASE = "BH6000";
        public const string USER_CONTROL_BASE = "BH5000";
        
        // WebInternalGuidelines
        public const string CONNECTION_HELPER_EXECUTE_QUERY = "BH10X4";
        public const string VALIDATION_HELPER_GET = "BH4001";
    }
}
