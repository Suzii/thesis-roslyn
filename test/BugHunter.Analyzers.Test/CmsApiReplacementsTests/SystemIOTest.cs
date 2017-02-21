using System.IO;
using System.Linq;
using BugHunter.Analyzers.CmsApiReplacementRules.Analyzers;
using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Analyzers.Test.CmsApiReplacementsTests
{
    [TestFixture]
    public class SystemIOTest : CodeFixVerifier<SystemIOAnalyzer>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return new MetadataReference[] {};
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestCase("using System.IO;\r\n", "Stream", "Stream.Null")]
        [TestCase("", "System.IO.Stream", "System.IO.Stream.Null")]
        public void OkInput_IOStream_NoDiagnostic(string usingDirectives, string returnInMethodSignature, string returnInMethodBody)
        {
            var test = $@"{usingDirectives}
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public {returnInMethodSignature} SampleMethod()
        {{ 
            return {returnInMethodBody};
        }}
    }}
}}";

            VerifyCSharpDiagnostic(test);
        }

        [TestCase("using System.IO;\r\n", @"IOException(""Ooops..."")")]
        [TestCase("using System.IO;\r\n", @"DirectoryNotFoundException(""Ooops..."")")]
        [TestCase("", @"System.IO.IOException(""Ooops..."")")]
        [TestCase("", @"System.IO.DirectoryNotFoundException(""Ooops..."")")]
        public void OkInput_IOException_NoDiagnostic(string usingDirectives, string exceptionInstance)
        {
            var test = $@"{usingDirectives}
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{ 
            throw new {exceptionInstance};
        }}
    }}
}}";

            VerifyCSharpDiagnostic(test);
        }

        [TestCase("using System.IO;\r\n", "BinaryReader", "new BinaryReader(Stream.Null)")]
        [TestCase("", "System.IO.BinaryReader", "new System.IO.BinaryReader(System.IO.Stream.Null)")]
        [TestCase("using System.IO;\r\n", "DirectoryInfo", @"new DirectoryInfo(""./some/path"")")]
        [TestCase("", "System.IO.DirectoryInfo", @"new System.IO.DirectoryInfo(""./some/path"")")]
        public void InputWithIncident_ForbiddenIdentifierUsed_SurfacesDiagnostic(string usingDirectives, string typeName, string objectInstantiation)
        {
            var test = $@"{usingDirectives}
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public {typeName} SampleMethod({typeName} param)
        {{ 
            var result = {objectInstantiation};
            {typeName} badVariable = param;
            return result;
        }}
    }}
}}";
            var usingLinesCount = usingDirectives.Count(c => c == '\n');
            var methodParameterColumn = 16 + 12 + 2 + typeName.Length;

            var returnTypeDiagnostic = CreateDiagnosticResult(typeName, 6 + usingLinesCount, 16);
            var methodParameterDiagnostic = CreateDiagnosticResult(typeName, 6 + usingLinesCount, methodParameterColumn);
            var objectInstantiationDiagnostic = CreateDiagnosticResult(objectInstantiation, 8 + usingLinesCount, 26);
            var explicitVariableDeclarationDiagnostic = CreateDiagnosticResult(typeName, 9 + usingLinesCount, 13);
            
            VerifyCSharpDiagnostic(test, returnTypeDiagnostic, methodParameterDiagnostic, objectInstantiationDiagnostic, explicitVariableDeclarationDiagnostic);
        }

        [TestCase("using System.IO;\r\n", "Path.DirectorySeparatorChar")]
        [TestCase("", "System.IO.Path.DirectorySeparatorChar")]
        public void InputWithIncident_StaticMember_SurfacesDiagnostic(string usingDirectives, string forbiddenUsage)
        {
            var test = $@"{usingDirectives}
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{ 
            var badVariable1 = {forbiddenUsage};
        }}
    }}
}}";
            var usingLinesCount = usingDirectives.Count(c => c == '\n');

            var expectedDiagnostic = CreateDiagnosticResult(forbiddenUsage, 8 + usingLinesCount, 32);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }

        [Test]
        public void InputWithIncident_ComplexNestedMemberAccess_SurfacesDiagnostic()
        {
            var test = @"
namespace SampleTestProject.CsSamples
{
    public class SampleClass
    {
        public static void HelperMethod(char c) 
        {
            // Do nothing
        }

        public void SampleMethod()
        {
            var directory = new System.IO.DirectoryInfo(System.IO.Path.GetFullPath(""./some/path""));
            SampleTestProject.CsSamples.SampleClass.HelperMethod(System.IO.Path.DirectorySeparatorChar);
        }
    }
}";
            var s= new System.IO.BinaryReader(Stream.Null);

            var objectCreationDiagnostic = CreateDiagnosticResult(@"new System.IO.DirectoryInfo(System.IO.Path.GetFullPath(""./some/path""))", 13, 29);
            var nestedInObjectCreationDiagnostic = CreateDiagnosticResult(@"System.IO.Path.GetFullPath(""./some/path"")", 13, 57);
            var nestedInMemberAccessDiagnostic = CreateDiagnosticResult("System.IO.Path.DirectorySeparatorChar", 14, 66);

            VerifyCSharpDiagnostic(test, objectCreationDiagnostic, nestedInObjectCreationDiagnostic, nestedInMemberAccessDiagnostic);
        }

        private static DiagnosticResult CreateDiagnosticResult(string messageArg, int row, int column)
        {
            return new DiagnosticResult
            {
                Id = DiagnosticIds.SYSTEM_IO,
                Message = $"'{messageArg}' should not use 'System.IO' directly. Use equivalent method from namespace 'CMS.IO'.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", row, column) }
            };
        }
    }
}