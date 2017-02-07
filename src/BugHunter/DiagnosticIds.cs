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
        public const string HTTP_REQUEST_COOKIES = "BH1003";
        public const string HTTP_RESPONSE_COOKIES = "BH1004";
        public const string HTTP_REQUEST_USER_HOST_ADDRESS = "BH1005";
        public const string HTTP_REQUEST_URL = "BH1006";
        public const string HTTP_REQUEST_BROWSER = "BH1007";

        public const string FORMS_AUTHENTICATION_SIGN_OUT = "BH1008";
        public const string PAGE_IS_POST_BACK = "BH1009";
        public const string PAGE_IS_CALLBACK = "BH1010";
        public const string CLIENT_SCRIPT_METHODS = "BH1011";
        public const string HTTP_RESPONSE_REDIRECT = "BH1012";
        public const string HTTP_REQUEST_QUERY_STRING = "BH1013";

        // ????
        public const string WHERE_LIKE_METHOD = "BH10X1";
        public const string EVENT_LOG_ARGUMENTS = "BH10X2";
        public const string LUCENE_SEARCH_DOCUMENT = "BH10X3";
        public const string CONNECTION_HELPER_EXECUTE_QUERY = "BH10X4";

        // String methods & Culture
        public const string STRING_MANIPULATION_METHODS = "BH2000";

        public const string STRING_EQUALS_METHOD = "BH2001";
        public const string STRING_COMPARE_TO_METHOD = "BH2002";

        public const string STRING_STARTS_ENDS_WITH_METHODS = "BH2003";
        public const string STRING_INDEX_OF_METHODS = "BH2004";

        public const string STRING_COMPARE_STATIC_METHOD = "BH2005";

        // System.IO
        public const string SYSTEM_IO = "BH3000";

        // WP checks
        public const string WEB_PART_BASE = "BH4000";
        public const string UI_WEB_PART_BASE = "BH4002";
        public const string VALIDATION_HELPER_GET = "BH4001";

        // Control checks
        public const string USER_CONTROL_BASE = "BH5000";
        
        // Page checks
        public const string PAGE_BASE = "BH6000";
    }
}
