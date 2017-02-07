namespace BugHunter.Test.Verifiers
{
    public class FakeFileInfo
    {
        public FakeFileInfo()
        {
            FileLoaction = "";
            FileNamePrefix = "Test";
            FileExtension = "cs";
        }

        public string FileExtension { get; set; }

        public string FileNamePrefix { get; set; }

        public string FileLoaction { get; set; }

        public string GetFullFilePath(int index) => $"{FileLoaction}{FileNamePrefix}{index}.{FileExtension}";
    }
}