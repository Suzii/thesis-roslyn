using BugHunter.TestUtils.Helpers;

namespace SampleProjectGenerator.CodeGenerators
{
    public interface IClassCodeGenerator
    {
        ProjectType ProjectType { get; }

        FakeFileInfo GetFakeFileInfo(int index);

        string[] GenerateClasses(int desiredNumberOfDiagnostics, int numberOfFiles);
    }
}