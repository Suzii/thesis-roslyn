using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ReportAnalyzerTimes.Aggregator
{
    /// <summary>
    /// Class that continuously preads content of file with given prefix and yields the contents of the file in form of lines
    /// </summary>
    internal class InputFilesProvider
    {
        private readonly string _fileNamePrefix;
        private readonly int _numberOfFiles;
        private readonly string _extension;

        public InputFilesProvider(string fileNamePrefix, int numberOfFiles, string extension)
        {
            _fileNamePrefix = fileNamePrefix;
            _numberOfFiles = numberOfFiles;
            _extension = extension;
        }

        public IEnumerable<IEnumerable<string>> GetLinesOfFiles()
        {
            return Enumerable.Range(1, _numberOfFiles)
                    .Select(ConstructFileName)
                    .Select(File.ReadAllLines);
        }

        private string ConstructFileName(int index)
        {
            return $"{_fileNamePrefix}{index}.{_extension}";
        }
    }
}