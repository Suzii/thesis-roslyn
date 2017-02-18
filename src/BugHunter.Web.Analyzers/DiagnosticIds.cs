namespace BugHunter.Web.Analyzers
{
    /// <summary>
    /// Class defining the IDs for diagnostic analyzers.
    /// </summary>
    internal static class DiagnosticIds
    {
        // CmsBaseClasses
        public const string WEB_PART_BASE = "BH3500";
        public const string UI_WEB_PART_BASE = "BH3501";
        public const string PAGE_BASE = "BH3502";
        public const string USER_CONTROL_BASE = "BH3503";
        
        // CmsApiGuideliness
        public const string VALIDATION_HELPER_GET = "BH2500";
        public const string CONNECTION_HELPER_EXECUTE_QUERY = "BH2501";
    }
}
