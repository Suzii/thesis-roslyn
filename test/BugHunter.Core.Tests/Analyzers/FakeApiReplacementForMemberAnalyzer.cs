using System.Collections.Immutable;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core.Tests.Analyzers
{
#pragma warning disable RS1001 // Missing diagnostic analyzer attribute. - only for test purposes, not a production analyzer
    public class FakeApiReplacementForMemberAnalyzer : DiagnosticAnalyzer
#pragma warning restore RS1001 // Missing diagnostic analyzer attribute.
    {
        public const string DiagnosticId = "BHFAKE";

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRulesProvider.GetRule(DiagnosticId, "FakeClass.FakeMember");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly ApiReplacementConfig ApiReplacementConfig = new ApiReplacementConfig(Rule,
            new []{ "FakeNamespace.FakeClass"},
            new []{ "FakeMember"});

        private static readonly ApiReplacementForMemberAnalyzer ApiReplacementAnalyzer = new ApiReplacementForMemberAnalyzer(ApiReplacementConfig);

        public override void Initialize(AnalysisContext context)
        {
            ApiReplacementAnalyzer.RegisterAnalyzers(context);
        }
    }
}