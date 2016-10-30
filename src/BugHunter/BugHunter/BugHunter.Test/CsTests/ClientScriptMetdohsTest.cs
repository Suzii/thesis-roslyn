//using System.Linq;
//using BugHunter.CsRules.Analyzers;
//using BugHunter.CsRules.CodeFixes;
//using BugHunter.Test.Shared;
//using BugHunter.Test.Verifiers;
//using Kentico.Google.Apis.Util;
//using Microsoft.CodeAnalysis;
//using NUnit.Framework;

//namespace BugHunter.Test.CsTests
//{
//    [TestFixture]
//    public class ClientScriptMethodsTest : CodeFixVerifier<ClientScriptMethodsAnalyzer>
//    {
//        protected override MetadataReference[] GetAdditionalReferences()
//        {
//            return ReferencesHelper.BasicReferences.Union(new[] { ReferencesHelper.SystemWebReference }).ToArray();
//        }

//        [Test]
//        public void EmptyInput_NoDiagnostic()
//        {
//            var test = @"";

//            VerifyCSharpDiagnostic(test);
//        }
        
//        [TestCase(@"RegisterArrayDeclaration(""arrayName"", ""arrayValue"")", "//TODO")]
//        [TestCase(@"RegisterClientScriptBlock(typeof(object), ""key"", ""script"")", "//TODO")]
//        [TestCase(@"RegisterClientScriptInclude(typeof(object), ""key"", ""url"")", "//TODO")]
//        [TestCase(@"RegisterStartupScript(typeof(object), ""key"", ""script"")", "//TODO")]
//        public void InputWithWhereLike_SimpleMemberAccess_SurfacesDiagnostic(string methodInvocation, string codeFix)
//        {
//            var test = $@"
//namespace SampleTestProject.CsSamples
//{{
//    public class SampleClass
//    {{
//        public void SampleMethod()
//        {{
//            var clientScript = new System.Web.UI.Page().ClientScript;
//            clientScript.{methodInvocation};
//        }}
//    }}
//}}";
//            var expectedDiagnostic = new DiagnosticResult
//            {
//                Id = DiagnosticIds.CLIENT_SCRIPT_METHODS,
//                Message = MessagesConstants.MESSAGE_NO_SUGGESTION.FormatString($"clientScript.{methodInvocation}"),
//                Severity = DiagnosticSeverity.Warning,
//                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
//            };

//            VerifyCSharpDiagnostic(test, expectedDiagnostic);

////            var expectedFix = $@"using CMS.Helpers;

////namespace SampleTestProject.CsSamples
////{{
////    public class SampleClass
////    {{
////        public void SampleMethod()
////        {{
////            var clientScript = new System.Web.UI.Page().ClientScript;
////            {codeFix};
////        }}
////    }}
////}}";
////            VerifyCSharpFix(test, expectedFix);
//        }

//        [TestCase(@"RegisterArrayDeclaration(""arrayName"", ""arrayValue"")", "//TODO")]
//        [TestCase(@"RegisterClientScriptBlock(typeof(object), ""key"", ""script"")", "//TODO")]
//        [TestCase(@"RegisterClientScriptInclude(typeof(object), ""key"", ""url"")", "//TODO")]
//        [TestCase(@"RegisterStartupScript(typeof(object), ""key"", ""script"")", "//TODO")]
//        public void InputWithIncident_ChainedMemberAccess_SurfacesDiagnostic(string methodInvocation, string codeFix)
//        {
//            var test = $@"
//namespace SampleTestProject.CsSamples
//{{
//    public class SampleClass
//    {{
//        public void SampleMethod()
//        {{
//            new System.Web.UI.Page().ClientScript.{methodInvocation};
//        }}
//    }}
//}}";
//            var expectedDiagnostic = new DiagnosticResult
//            {
//                Id = DiagnosticIds.CLIENT_SCRIPT_METHODS,
//                Message = MessagesConstants.MESSAGE_NO_SUGGESTION.FormatString($"new System.Web.UI.Page().ClientScript.{methodInvocation}"),
//                Severity = DiagnosticSeverity.Warning,
//                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 13) }
//            };

//            VerifyCSharpDiagnostic(test, expectedDiagnostic);

////            var expectedFix = $@"using CMS.Helpers;

////namespace SampleTestProject.CsSamples
////{{
////    public class SampleClass
////    {{
////        public void SampleMethod()
////        {{
////            {codeFix};
////        }}
////    }}
////}}";
////            VerifyCSharpFix(test, expectedFix);
//        }
//    }
//}