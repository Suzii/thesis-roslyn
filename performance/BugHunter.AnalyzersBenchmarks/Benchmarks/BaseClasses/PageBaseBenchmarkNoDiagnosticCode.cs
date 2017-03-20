using System.Linq;
using BenchmarkDotNet.Attributes;
using BugHunter.AnalyzersBenchmarks.Benchmarks.BaseClasses.Analyzers;
using BugHunter.TestUtils;
using BugHunter.TestUtils.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using SampleProjectGenerator.CodeGenerators.BaseClassRules.Implementation;

namespace BugHunter.AnalyzersBenchmarks.Benchmarks.BaseClasses
{
    public class PageBaseBenchmarkNoDiagnosticCode
    {
        //private readonly DiagnosticAnalyzer _v0 = new SyntaxNodeClassDeclarationBaselineAnalyzer();
        private readonly DiagnosticAnalyzer _v1 = new PageBaseAnalyzer_V1_SyntaxTree();
        private readonly DiagnosticAnalyzer _v2 = new PageBaseAnalyzer_V2_NamedTypeSymbol();
        private readonly DiagnosticAnalyzer _v3 = new PageBaseAnalyzer_V3_NamedTypeSymbolToString();
        private readonly DiagnosticAnalyzer _v4 = new PageBaseAnalyzer_V4_NamedTypeSymbolToStringWithoutCOmpilationStart();

        private readonly MetadataReference[] _additionalReferences;
        private readonly string[] _sources;
        private readonly FakeFileInfo _fakeFileInfo;

        public PageBaseBenchmarkNoDiagnosticCode()
        {
            _additionalReferences = ReferencesHelper.CMSBasicReferences
                .Union(new[] { ReferencesHelper.CMSBaseWebUI, ReferencesHelper.SystemWebReference, ReferencesHelper.SystemWebUIReference, ReferencesHelper.CMSUiControls })
                .ToArray();

            var codeGenerator = new UserControlBase();//new PageBase();
            _sources = codeGenerator.GenerateClasses(1000, 500).Union(new WebPartBase().GenerateClasses(1000, 500)).ToArray();
            _fakeFileInfo = codeGenerator.GetFakeFileInfo(0);
        }

        [Benchmark]
        public int AnalyzerV1_SyntxNodeRegistered()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences, _fakeFileInfo, _v1);
        }

        [Benchmark]
        public int AnalyzerV2_NamedTypeSymbolRegistered()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences, _fakeFileInfo, _v2);
        }

        [Benchmark]
        public int AnalyzerV3__NamedTypeToString()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences, _fakeFileInfo, _v3);
        }

        [Benchmark]
        public int AnalyzerV4_NamedTypeToStringWithoutCompilationStart()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences, _fakeFileInfo, _v4);
        }
    }
}