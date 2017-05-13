// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using BugHunter.Analyzers.CmsApiReplacementRules.Analyzers;
using BugHunter.Analyzers.CmsApiReplacementRules.CodeFixes;
using BugHunter.Analyzers.Test.CmsApiReplacementsTests.Constants;
using BugHunter.Core.Constants;
using BugHunter.TestUtils;
using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Analyzers.Test.CmsApiReplacementsTests
{
    [TestFixture]
    public class HttpRequestBrowserTest : CodeFixVerifier<HttpRequestBrowserAnalyzer, HttpRequestBrowserCodeFixProvider>
    {
        protected override MetadataReference[] AdditionalReferences
            => ReferencesHelper.CMSBasicReferences.Union(new[] { ReferencesHelper.SystemWebReference }).ToArray();

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = string.Empty;

            VerifyCSharpDiagnostic(test);
        }

        [TestCase(@"new System.Web.HttpRequest(""fileName"", ""url"", ""queryString"")")]
        [TestCase(@"new System.Web.HttpRequestWrapper(new System.Web.HttpRequest(""fileName"", ""url"", ""queryString""))")]
        public void InputWithIncident_ChainedMemberAccess_SurfacesDiagnostic(string requestInstance)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var request = {requestInstance};
            var browser = request.Browser.Browser;
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult("request.Browser.Browser", "BrowserHelper.GetBrowser()").WithLocation(9, 27);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var request = {requestInstance};
            var browser = BrowserHelper.GetBrowser();
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }

        [TestCase(@"new System.Web.HttpRequest(""fileName"", ""url"", ""queryString"")")]
        [TestCase(@"new System.Web.HttpRequestWrapper(new System.Web.HttpRequest(""fileName"", ""url"", ""queryString""))")]
        public void InputWithIncident_SimpleMemberAccess_SurfacesDiagnostic(string requestInstance)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var request = {requestInstance};
            var browserInfo = request.Browser;
            var browser = browserInfo.Browser;
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult("browserInfo.Browser", "BrowserHelper.GetBrowser()").WithLocation(10, 27);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var request = {requestInstance};
            var browserInfo = request.Browser;
            var browser = BrowserHelper.GetBrowser();
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }

        [TestCase(@"new System.Web.HttpRequest(""fileName"", ""url"", ""queryString"")")]
        [TestCase(@"new System.Web.HttpRequestWrapper(new System.Web.HttpRequest(""fileName"", ""url"", ""queryString""))")]
        public void InputWithIncident_FollowUpMemberAccess_SurfacesDiagnostic(string requestInstance)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var request = {requestInstance};
            var browserInfo = request.Browser;
            var browser = browserInfo.Browser.Contains(""Ooops..."");
        }}
    }}
}}";

            var expectedDiagnostic = CreateDiagnosticResult("browserInfo.Browser", "BrowserHelper.GetBrowser()").WithLocation(10, 27);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var request = {requestInstance};
            var browserInfo = request.Browser;
            var browser = BrowserHelper.GetBrowser().Contains(""Ooops..."");
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }

        [TestCase(@"new System.Web.HttpRequest(""fileName"", ""url"", ""queryString"")")]
        [TestCase(@"new System.Web.HttpRequestWrapper(new System.Web.HttpRequest(""fileName"", ""url"", ""queryString""))")]
        public void InputWithIncident_ConditionalAccess_SurfacesDiagnostic_NoCodeFix(string requestInstance)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var request = {requestInstance};
            var browserInfo = request.Browser;
            var browser = browserInfo?.Browser.Contains(""Ooops..."");
        }}
    }}
}}";

            var expectedDiagnostic = CreateDiagnosticResult("browserInfo?.Browser", "BrowserHelper.GetBrowser()").WithLocation(10, 27);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            VerifyCSharpFix(test, test);
        }

        private static DiagnosticResult CreateDiagnosticResult(params object[] messageArgs)
            => new DiagnosticResult
            {
                Id = DiagnosticIds.HttpRequestBrowser,
                Message = string.Format(MessagesConstants.Message, messageArgs),
                Severity = DiagnosticSeverity.Warning,
            };
    }
}