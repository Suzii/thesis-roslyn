using BugHunter.CsRules.Analyzers;
using BugHunter.CsRules.CodeFixes;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.CsTests
{    
    [TestFixture]
    // TODO test codefix as well
    public class ModuleRegistrationTest : CodeFixVerifier<ModuleRegistrationAnalyzer>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return ReferencesHelper.BasicReferences;
        }

        private DiagnosticResult GetDiagnosticResult(params string[] messageArgumentStrings)
        {
            return new DiagnosticResult
            {
                Id = DiagnosticIds.MODULE_REGISTRATION,
                Message = string.Format("Module or ModuleEntry '{0}' is not registered in the same file where it is declared. Add assembly attribute [assembly: RegisterModule(typeof({0}))] to the file.", messageArgumentStrings),
                Severity = DiagnosticSeverity.Warning,
            };
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void OkInput_ClassDoesNotContainModule_NoDiagnostic()
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass
    {{
    }}
}}";
            VerifyCSharpDiagnostic(test);
        }
        
        [Test]
        public void OkayInput_ModuleIsRegistered_NoDiagnostic()
        {
            var test = @"using CMS;
using CMS.Core;
using SampleTestProject.CsSamples;

[assembly: RegisterModule(typeof(MyModule))]
namespace SampleTestProject.CsSamples
{
    public class MyModule : CMS.DataEngine.Module
    {
        public MyModule(ModuleMetadata metadata, bool isInstallable = false) : base(metadata, isInstallable)
        {
        }

        public MyModule(string moduleName, bool isInstallable = false) : base(moduleName, isInstallable)
        {
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void OkayInput_ModuleEntryIsRegistered_NoDiagnostic()
        {
            var test = @"using CMS;
using CMS.Core;
using SampleTestProject.CsSamples;

[assembly: RegisterModule(typeof(MyModule))]
namespace SampleTestProject.CsSamples
{
    public class MyModule : ModuleEntry
    {
        public MyModule(ModuleMetadata metadata, bool isInstallable = false) : base(metadata, isInstallable)
        {
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void InputWithError_ModuleNotRegistered_SurfacesDiagnostic()
        {
            var test = @"using CMS;
using CMS.Core;

namespace SampleTestProject.CsSamples
{
    public class MyModule : CMS.DataEngine.Module
    {
        public MyModule(ModuleMetadata metadata, bool isInstallable = false) : base(metadata, isInstallable)
        {
        }

        public MyModule(string moduleName, bool isInstallable = false) : base(moduleName, isInstallable)
        {
        }
    }
}";
            var expectedDiagnostic = GetDiagnosticResult("MyModule").WithLocation(6, 18);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = @"using CMS;
using CMS.Core;
using SampleTestProject.CsSamples;

[assembly: RegisterModule(typeof(MyModule))]
namespace SampleTestProject.CsSamples
{
    public class MyModule : CMS.DataEngine.Module
    {
        public MyModule(ModuleMetadata metadata, bool isInstallable = false) : base(metadata, isInstallable)
        {
        }

        public MyModule(string moduleName, bool isInstallable = false) : base(moduleName, isInstallable)
        {
        }
    }
}";

            //VerifyCSharpFix(test, expectedFix);
        }

        [Test]
        public void InputWithError_ModuleEntryNotRegistered_SurfacesDiagnostic()
        {
            var test = @"using CMS.Core;

namespace SampleTestProject.CsSamples
{
    public class MyModule : ModuleEntry
    {
        public MyModule(ModuleMetadata metadata, bool isInstallable = false) : base(metadata, isInstallable)
        {
        }
    }
}";
            var expectedDiagnostic = GetDiagnosticResult("MyModule").WithLocation(5, 18);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = @"using CMS;
using CMS.Core;
using SampleTestProject.CsSamples;

[assembly: RegisterModule(typeof(MyModule))]
namespace SampleTestProject.CsSamples
{
    public class MyModule : ModuleEntry
    {
        public MyModule(ModuleMetadata metadata, bool isInstallable = false) : base(metadata, isInstallable)
        {
        }
    }
}";

            //VerifyCSharpFix(test, expectedFix);
        }

        [Test]
        public void InputWithError_WrongModuleRegistered_SurfacesDiagnostic()
        {
            var test = @"using CMS;
using CMS.Core;

[assembly: CMS.RegisterModule(typeof(System.String))]
namespace SampleTestProject.CsSamples
{
    public class MyModule : CMS.DataEngine.Module
    {
        public MyModule(ModuleMetadata metadata, bool isInstallable = false) : base(metadata, isInstallable)
        {
        }

        public MyModule(string moduleName, bool isInstallable = false) : base(moduleName, isInstallable)
        {
        }
    }
}";
            var expectedDiagnostic = GetDiagnosticResult("MyModule").WithLocation(7, 18);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = @"using CMS;
using CMS.Core;
using SampleTestProject.CsSamples;

[assembly: CMS.RegisterModule(typeof(System.String))]
[assembly: RegisterModule(typeof(MyModule))]
namespace SampleTestProject.CsSamples
{
    public class MyModule : CMS.DataEngine.Module
    {
        public MyModule(ModuleMetadata metadata, bool isInstallable = false) : base(metadata, isInstallable)
        {
        }

        public MyModule(string moduleName, bool isInstallable = false) : base(moduleName, isInstallable)
        {
        }
    }
}";

            //VerifyCSharpFix(test, expectedFix);
        }
    }
}