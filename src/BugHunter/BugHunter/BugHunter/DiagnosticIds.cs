namespace BugHunter
{
    /// <summary>
    /// Class defining the IDs for diagnostic analyzers.
    /// </summary>
    internal static class DiagnosticIds
    {
        // CsRules
        // API Replacements
        public const string HTTP_SESSION_SESSION_ID = "BH1000";
        public const string HTTP_SESSION_ELEMENT_ACCESS_GET = "BH1001";
        public const string HTTP_SESSION_ELEMENT_ACCESS_SET = "BH1002";
        public const string HTTP_REQUEST_AND_RESPONSE_COOKIE = "BH1003";
        public const string HTTP_RESPONSE_REDIRECT = "BH1004";
        public const string HTTP_REQUEST_USER_HOST_ADDRESS = "BH1005";
        public const string HTTP_REQUEST_URL = "BH1006";
        public const string HTTP_REQUEST_BROWSER = "BH1007";

        // ????
        public const string WHERE_LIKE_METHOD = "BH10X1";
        public const string EVENT_LOG_ARGUMENTS = "BH10X2";
    }
}
