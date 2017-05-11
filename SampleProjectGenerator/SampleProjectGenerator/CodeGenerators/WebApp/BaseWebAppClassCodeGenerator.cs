namespace SampleProjectGenerator.CodeGenerators.WebApp
{
    public abstract class BaseWebAppClassCodeGenerator : BaseClassCodeGenerator
    {
        public override ProjectType ProjectType { get; } = ProjectType.WebApp;

        protected override string GetClassPrefix(int index)
        {
            return $@"namespace WebApp
{{
    public class {GetFakeFileInfo(index).FileName}{index}
    {{";
        }

        protected override string GetClassSuffix()
        {
            return @"
    }
}";
        }
    }
}