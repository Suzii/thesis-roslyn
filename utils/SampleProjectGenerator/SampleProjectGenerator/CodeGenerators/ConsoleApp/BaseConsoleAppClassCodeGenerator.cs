namespace SampleProjectGenerator.CodeGenerators.ConsoleApp
{
    public abstract class BaseConsoleAppClassCodeGenerator : BaseClassCodeGenerator
    {
        public override ProjectType ProjectType { get; } = ProjectType.ConsoleApp;

        protected override string GetClassPrefix(int index)
        {
            return $@"namespace ConsoleApp
{{
    public class {GetFakeFileInfo(index).FileName}
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