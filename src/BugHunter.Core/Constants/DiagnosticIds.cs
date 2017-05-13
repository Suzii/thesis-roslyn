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
        public const string LuceneSearchDocument = "BH0000";

        // CmsApiReplacements
        public const string HttpSessionSessionId = "BH1000";
        public const string HttpSessionElementAccessGet = "BH1001";
        public const string HttpSessionElementAccessSet = "BH1002";
        public const string HttpRequestCookies = "BH1003";
        public const string HttpResponseCookies = "BH1004";
        public const string HttpRequestUserHostAddress = "BH1005";
        public const string HttpRequestUrl = "BH1006";
        public const string HttpRequestBrowser = "BH1007";
        public const string HttpResponseRedirect = "BH1008";
        public const string HttpRequestQueryString = "BH1009";

        public const string PageIsCallback = "BH1010";
        public const string PageIsPostBack = "BH1011";
        public const string FormsAuthenticationSignOut = "BH1012";
        public const string ClientScriptMethods = "BH1013";

        public const string SystemIO = "BH1014";

        // CmsApiGuidelines
        public const string WhereLikeMethod = "BH2000";
        public const string EventLogArguments = "BH2001";

        public const string ValidationHelperGet = "BH2500";
        public const string ConnectionHelperExecuteQuery = "BH2501";

        // CmsBaseClasses
        public const string ModuleRegistration = "BH3000";

        public const string WebPartBase = "BH3500";
        public const string UIWebPartBase = "BH3501";
        public const string PageBase = "BH3502";
        public const string UserControlBase = "BH3503";

        // StringAndCulture
        public const string StringManipulationMethods = "BH4000";
        public const string StringEqualsMethod = "BH4001";
        public const string StringCompareToMethod = "BH4002";
        public const string StringStartsEndsWithMethods = "BH4003";
        public const string StringIndexOfMethods = "BH4004";
        public const string StringCompareStaticMethod = "BH4005";
    }
}
