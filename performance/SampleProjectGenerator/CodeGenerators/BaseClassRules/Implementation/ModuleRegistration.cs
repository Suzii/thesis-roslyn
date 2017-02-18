using BugHunter.TestUtils.Helpers;

namespace SampleProjectGenerator.CodeGenerators.BaseClassRules.Implementation
{
    public class ModuleRegistration : BaseForBaseClassesCodeGenerator
    {
        public override FakeFileInfo GetFakeFileInfo(int index)
        {
            return new FakeFileInfo(nameof(ModuleRegistration), index)
            {
                FileLocation = "Modules"
            };
        }

        protected override string GenerateSingleClass(int index)
        {
            return $@"
    public class ModuleRegistration{index} : CMS.DataEngine.Module
    {{
        public MyModule(CMS.Core.ModuleMetadata metadata, bool isInstallable = false) : base(metadata, isInstallable)
        {{
        }}

        public MyModule(string moduleName, bool isInstallable = false) : base(moduleName, isInstallable)
        {{
        }}
    }}";
        }
    }
}