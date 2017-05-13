using System;
using System.Collections.Generic;
using System.Linq;
using BugHunter.TestUtils.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.Text;

namespace BugHunter.TestUtils.Services
{
    /// <summary>
    /// Class for turning strings into documents and getting the diagnostics on them
    /// All methods are static
    /// </summary>
    public static class ProjectCompilation
    {
        internal static string TestProjectName = "TestProject";
        internal static FakeFileInfo DefaultFileInfo = new FakeFileInfo { FileLocation = "", FileName = "Test", FileExtension = "cs" };

        #region Set up compilation and documents
        /// <summary>
        /// Get the existing compiler diagnostics on the inputted document.
        /// </summary>
        /// <param name="document">The Document to run the compiler diagnostic analyzers on</param>
        /// <returns>The compiler diagnostics that were found in the code</returns>
        public static IEnumerable<Diagnostic> GetCompilerDiagnostics(Document document)
        {
            return document.GetSemanticModelAsync().Result.GetDiagnostics();
        }

        /// <summary>
        /// Given an array of strings as sources, turn them into a project and return the documents and spans of it.
        /// </summary>
        /// <param name="sources">Classes in the form of strings</param>
        /// <param name="references">Array of additional types source files have dependencies on</param>
        /// <param name="fakeFileInfo">Fake file info for generated documents</param>
        /// <returns>Documents produced from the sources</returns>
        public static Document[] GetDocuments(string[] sources, MetadataReference[] references, FakeFileInfo fakeFileInfo)
        {
            var project = CreateProject(sources, references, fakeFileInfo);
            var documents = project.Documents.ToArray();

            if (sources.Length != documents.Length)
            {
                throw new SystemException("Amount of sources did not match amount of Documents created");
            }

            return documents;
        }

        /// <summary>
        /// Create a Document from a string through creating a project that contains it.
        /// </summary>
        /// <param name="source">Classes in the form of a string</param>
        /// <param name="references">Array of additional types source file has dependency on</param>
        /// <param name="fakeFileInfo">Fake file info for generated document</param>
        /// <returns>A Document created from the source string</returns>
        public static Document CreateDocument(string source, MetadataReference[] references, FakeFileInfo fakeFileInfo)
        {
            return CreateProject(new[] { source }, references, fakeFileInfo).Documents.First();
        }

        /// <summary>
        /// Given a document, turn it into a string based on the syntax root
        /// </summary>
        /// <param name="document">The Document to be converted to a string</param>
        /// <returns>A string containing the syntax of the Document after formatting</returns>
        public static string GetStringFromDocument(Document document)
        {
            var simplifiedDoc = Simplifier.ReduceAsync(document, Simplifier.Annotation).Result;
            var root = simplifiedDoc.GetSyntaxRootAsync().Result;
            root = Formatter.Format(root, Formatter.Annotation, simplifiedDoc.Project.Solution.Workspace);
            return root.GetText().ToString();
        }

        /// <summary>
        /// Create a project using the inputted strings as sources.
        /// </summary>
        /// <param name="sources">Classes in the form of strings</param>
        /// <param name="references">Additional references to be added to project</param>
        /// <param name="fakeFileInfo">Fake file info for generated documents</param>
        /// <returns>A Project created out of the Documents created from the source strings</returns>
        public static Project CreateProject(string[] sources, MetadataReference[] references, FakeFileInfo fakeFileInfo)
        {
            var solution = CreateSolutionWithSingleProject(sources, references, fakeFileInfo);

            return solution.Projects.SingleOrDefault();
        }

        public static Solution CreateSolutionWithSingleProject(string[] sources, MetadataReference[] references, FakeFileInfo fakeFileInfo)
        {
            fakeFileInfo = fakeFileInfo ?? DefaultFileInfo;
            var projectId = ProjectId.CreateNewId(debugName: TestProjectName);

            var solution = new AdhocWorkspace()
                .CurrentSolution
                .AddProject(projectId, TestProjectName, TestProjectName, LanguageNames.CSharp)
                .WithProjectCompilationOptions(projectId, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddMetadataReferences(projectId, ReferencesHelper.CoreDotNetReferences);

            if (references != null)
            {
                solution = solution.AddMetadataReferences(projectId, references.Distinct());
            }

            var count = 0;
            foreach (var source in sources)
            {
                var newFileName = fakeFileInfo.GetFullFilePath(count++);
                var documentId = DocumentId.CreateNewId(projectId, debugName: newFileName);
                var sourceText = SourceText.From(source);

                solution = solution.AddDocument(documentId, newFileName, sourceText);
            }

            return solution;
        }
    #endregion
    }
}