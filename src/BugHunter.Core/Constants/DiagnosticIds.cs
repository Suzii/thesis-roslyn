namespace BugHunter.Core.Constants
{
    /// <summary>
    /// Class defining the IDs for all diagnostic analyzers.
    /// </summary>
    /// <remarks>
    /// No not move to spearate files/projects in order to ensure no duplicate identifiers are assigned
    /// </remarks>
    public static class DiagnosticIds
    {
        // AbstractionOverImplementation
        public const string LUCENE_SEARCH_DOCUMENT = "BH0000";
        
        // CmsApiReplacements
        public const string HTTP_SESSION_SESSION_ID = "BH1000";
        public const string HTTP_SESSION_ELEMENT_ACCESS_GET = "BH1001";
        public const string HTTP_SESSION_ELEMENT_ACCESS_SET = "BH1002";
        public const string HTTP_REQUEST_COOKIES = "BH1003";
        public const string HTTP_RESPONSE_COOKIES = "BH1004";
        public const string HTTP_REQUEST_USER_HOST_ADDRESS = "BH1005";
        public const string HTTP_REQUEST_URL = "BH1006";
        public const string HTTP_REQUEST_BROWSER = "BH1007";
        public const string HTTP_RESPONSE_REDIRECT = "BH1008";
        public const string HTTP_REQUEST_QUERY_STRING = "BH1009";

        public const string PAGE_IS_CALLBACK = "BH1010";
        public const string PAGE_IS_POST_BACK = "BH1011";
        public const string FORMS_AUTHENTICATION_SIGN_OUT = "BH1012";
        public const string CLIENT_SCRIPT_METHODS = "BH1013";

        public const string SYSTEM_IO = "BH1014";

        // CmsApiGuidelines
        public const string WHERE_LIKE_METHOD = "BH2000";
        public const string EVENT_LOG_ARGUMENTS = "BH2001";

        public const string VALIDATION_HELPER_GET = "BH2500";
        public const string CONNECTION_HELPER_EXECUTE_QUERY = "BH2501";

        // CmsBaseClasses
        public const string MODULE_REGISTRATION = "BH3000";
        
        public const string WEB_PART_BASE = "BH3500";
        public const string UI_WEB_PART_BASE = "BH3501";
        public const string PAGE_BASE = "BH3502";
        public const string USER_CONTROL_BASE = "BH3503";

        // StringAndCulture
        public const string STRING_MANIPULATION_METHODS = "BH4000";
        public const string STRING_EQUALS_METHOD = "BH4001";
        public const string STRING_COMPARE_TO_METHOD = "BH4002";
        public const string STRING_STARTS_ENDS_WITH_METHODS = "BH4003";
        public const string STRING_INDEX_OF_METHODS = "BH4004";
        public const string STRING_COMPARE_STATIC_METHOD = "BH4005";
    }
}
