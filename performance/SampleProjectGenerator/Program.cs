﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SampleProjectGenerator.CodeGenerators;

namespace SampleProjectGenerator
{
    internal class Program
    {
        private static readonly int DesiredNumberOfDiagnosticsTotal = 10;

        private static readonly int NumberOfFilesPerAnalyzer = 2;
        
        private static void Main(string[] args)
        {
            var sampleProjectFolder = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\..\thesis-sample-test-project\SampleProject"));
            if (!Directory.Exists(sampleProjectFolder))
            {
                Console.WriteLine("This console app expects you have https://github.com/Suzii/thesis-sample-test-project cloned in a folder next to thesis-roslyn project");
                Console.WriteLine("Run git clone to create desired folder structure in " + sampleProjectFolder);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Generating project...");
            var classCodeGenerators = GetAllClassCodeGenerators();

            foreach (var codeGenerator in classCodeGenerators)
            {
                var generatedClasses = codeGenerator.GenerateClasses(DesiredNumberOfDiagnosticsTotal, NumberOfFilesPerAnalyzer);

                for (int i = 0; i < NumberOfFilesPerAnalyzer; i++)
                {
                    var documentPath = Path.Combine(sampleProjectFolder, RelativeDocumentPaths.Get(codeGenerator.ProjectType), codeGenerator.GetFakeFileInfo(i).FullFilePath);
                    File.WriteAllText(documentPath, generatedClasses[i]);
                }
            }

            Console.WriteLine($"Project files generated to {sampleProjectFolder}");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static IEnumerable<IClassCodeGenerator> GetAllClassCodeGenerators()
        {
            var generatorInterface = typeof(IClassCodeGenerator);
            var codeGenerators = generatorInterface.Assembly
                .GetTypes()
                .Where(type => generatorInterface.IsAssignableFrom(type) && !type.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<IClassCodeGenerator>();

            return codeGenerators;
        }
    }
}