namespace BugHunter.Test.Verifiers
{
    public class FakeFileInfo
    {
        public FakeFileInfo()
        {
            FilePath = "";
            FileNamePrefix = "Test";
            FileExtension = "cs";
        }

        public string FileExtension { get; set; }

        public string FileNamePrefix { get; set; }

        public string FilePath { get; set; }
    }
}