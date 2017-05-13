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
    public class ClientScriptMethodsTest : CodeFixVerifier<ClientScriptMethodsAnalyzer, ClientScriptMethodsCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
            => ReferencesHelper.CMSBasicReferences.Union(new[] { ReferencesHelper.SystemWebReference, ReferencesHelper.CMSBaseWebUI }).ToArray();

        private static DiagnosticResult CreateDiagnosticResult(params object[] messageArgs)
            => new DiagnosticResult
            {
                Id = DiagnosticIds.ClientScriptMethods,
                Message = string.Format(MessagesConstants.MessageNoSuggestion, messageArgs),
                Severity = DiagnosticSeverity.Warning,
            };

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestCase(@"RegisterArrayDeclaration(""arrayName"", ""arrayValue"")", @"RegisterArrayDeclaration(this, ""arrayName"", ""arrayValue"")")]
        [TestCase(@"RegisterClientScriptBlock(typeof(object), ""key"", ""script"")", @"RegisterClientScriptBlock(this, typeof(object), ""key"", ""script"")")]
        [TestCase(@"RegisterClientScriptInclude(typeof(object), ""key"", ""url"")", @"RegisterClientScriptInclude(this, typeof(object), ""key"", ""url"")")]
        [TestCase(@"RegisterStartupScript(typeof(object), ""key"", ""script"")", @"RegisterStartupScript(this, typeof(object), ""key"", ""script"")")]
        public void InputWithWhereLike_SimpleMemberAccess_SurfacesDiagnostic(string methodInvocation, string codeFix)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass : System.Web.UI.Control
    {{
        public void SampleMethod()
        {{
            var clientScript = new System.Web.UI.Page().ClientScript;
            clientScript.{methodInvocation};
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult($"clientScript.{methodInvocation}").WithLocation(9, 13);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Base.Web.UI;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass : System.Web.UI.Control
    {{
        public void SampleMethod()
        {{
            var clientScript = new System.Web.UI.Page().ClientScript;
            ScriptHelper.{codeFix};
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }

        [TestCase(@"RegisterArrayDeclaration(""arrayName"", ""arrayValue"")", @"RegisterArrayDeclaration(this, ""arrayName"", ""arrayValue"")")]
        [TestCase(@"RegisterClientScriptBlock(typeof(object), ""key"", ""script"")", @"RegisterClientScriptBlock(this, typeof(object), ""key"", ""script"")")]
        [TestCase(@"RegisterClientScriptInclude(typeof(object), ""key"", ""url"")", @"RegisterClientScriptInclude(this, typeof(object), ""key"", ""url"")")]
        [TestCase(@"RegisterStartupScript(typeof(object), ""key"", ""script"")", @"RegisterStartupScript(this, typeof(object), ""key"", ""script"")")]
        public void InputWithIncident_ChainedMemberAccess_SurfacesDiagnostic(string methodInvocation, string codeFix)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass : System.Web.UI.Control
    {{
        public void SampleMethod()
        {{
            new System.Web.UI.Page().ClientScript.{methodInvocation};
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult($"new System.Web.UI.Page().ClientScript.{methodInvocation}").WithLocation(8, 13);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Base.Web.UI;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass : System.Web.UI.Control
    {{
        public void SampleMethod()
        {{
            ScriptHelper.{codeFix};
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }

        [TestCase(@"RegisterArrayDeclaration(""arrayName"", ""arrayValue"")")]
        [TestCase(@"RegisterClientScriptBlock(typeof(object), ""key"", ""script"")")]
        [TestCase(@"RegisterClientScriptInclude(typeof(object), ""key"", ""url"")")]
        [TestCase(@"RegisterStartupScript(typeof(object), ""key"", ""script"")")]
        public void InputWithIncident_SurfacesDiagnostic_NoCodeFixProvided(string methodInvocation)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            new System.Web.UI.Page().ClientScript.{methodInvocation};
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult($"new System.Web.UI.Page().ClientScript.{methodInvocation}").WithLocation(8, 13);
            
            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            // SampleClass does not inherit from System.Web.UI.Control, verify no codefix is applied
            VerifyCSharpFix(test, test);
        }

        [TestCase(@"RegisterArrayDeclaration(""arrayName"", ""arrayValue"")")]
        [TestCase(@"RegisterClientScriptBlock(typeof(object), ""key"", ""script"")")]
        [TestCase(@"RegisterClientScriptInclude(typeof(object), ""key"", ""url"")")]
        [TestCase(@"RegisterStartupScript(typeof(object), ""key"", ""script"")")]
        public void InputWithIncidentConditionalAccess_SurfacesDiagnostic_NoCodeFixProvided(string methodInvocation)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass : System.Web.UI.Control
    {{
        public void SampleMethod()
        {{
            new System.Web.UI.Page().ClientScript?.{methodInvocation};
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult($".{methodInvocation}").WithLocation(8, 51);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            // verify no codefix is applied
            VerifyCSharpFix(test, test);
        }
    }
}