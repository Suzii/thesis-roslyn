// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ReportAnalyzerTimes.Aggregator
{
    /// <summary>
    /// Class that continuously reads the contents of a file with given prefix and yields the contents of the file in form of lines
    /// </summary>
    internal class InputFilesProvider
    {
        private readonly string _fileNamePrefix;
        private readonly int _numberOfFiles;
        private readonly string _extension;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputFilesProvider"/> class.
        /// </summary>
        /// <param name="fileNamePrefix">Prefix of the file name</param>
        /// <param name="numberOfFiles">Number of files</param>
        /// <param name="extension">File extension</param>
        public InputFilesProvider(string fileNamePrefix, int numberOfFiles, string extension)
        {
            _fileNamePrefix = fileNamePrefix;
            _numberOfFiles = numberOfFiles;
            _extension = extension;
        }

        /// <summary>
        /// Return all lines from all files
        /// </summary>
        /// <returns>All lines of all files</returns>
        public IEnumerable<IEnumerable<string>> GetLinesOfFiles()
        {
            return Enumerable.Range(0, _numberOfFiles)
                    .Select(ConstructFileName)
                    .Select(File.ReadAllLines);
        }

        private string ConstructFileName(int index)
        {
            return $"{_fileNamePrefix}{index}.{_extension}";
        }
    }
}