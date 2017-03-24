using System.Collections.Immutable;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Helpers.DiagnosticDescriptionBuilders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core.Tests.Analyzers
{
    public class FakeApiReplacementForMemberAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = "FAKE_ID";

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRuleBuilder.GetRule(DIAGNOSTIC_ID, "FakeClass.FakeMember");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly ApiReplacementConfig apiReplacementConfig = new ApiReplacementConfig(
                    Rule,
                    ImmutableHashSet.Create("FakeNamespace.FakeClass"),
                    ImmutableHashSet.Create("FakeMember"));

        private static readonly ApiReplacementForMemberAnalyzer apiReplacementAnalyzer = new ApiReplacementForMemberAnalyzer(apiReplacementConfig);

        public override void Initialize(AnalysisContext context)
        {
            apiReplacementAnalyzer.RegisterAnalyzers(context);
        }
    }
}