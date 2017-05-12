using System.Collections.Immutable;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core.Tests.Analyzers
{
#pragma warning disable RS1001 // Missing diagnostic analyzer attribute. - only for test purposes, not a production analyzer
    public class FakeApiReplacementForMethodAnalyzer : DiagnosticAnalyzer
#pragma warning restore RS1001 // Missing diagnostic analyzer attribute.
    {
        public const string DIAGNOSTIC_ID = "BHFAKE";

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRulesProvider.GetRule(DIAGNOSTIC_ID, "FakeClass.FakeMethod");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly ApiReplacementConfig apiReplacementConfig = new ApiReplacementConfig(Rule,
            new []{ "FakeNamespace.FakeClass"},
            new []{ "FakeMethod"});

        private static readonly ApiReplacementForMethodAnalyzer apiReplacementAnalyzer = new ApiReplacementForMethodAnalyzer(apiReplacementConfig);

        public override void Initialize(AnalysisContext context)
        {
            apiReplacementAnalyzer.RegisterAnalyzers(context);
        }
    }
}