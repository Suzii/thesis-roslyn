using System.Collections.Immutable;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Constants;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiReplacementRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ClientScriptMethodsAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raised by <see cref="ClientScriptMethodsAnalyzer"/>
        /// </summary>
        public const string DiagnosticId = DiagnosticIds.ClientScriptMethods;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRulesProvider.GetRule(DiagnosticId, "ClientScript methods");

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly ApiReplacementConfig Config = new ApiReplacementConfig(Rule,
            new[] { "System.Web.UI.ClientScriptManager" },
            new[] { "RegisterArrayDeclaration", "RegisterClientScriptBlock", "RegisterClientScriptInclude", "RegisterStartupScript" });

        private static readonly ApiReplacementForMethodAnalyzer Analyzer = new ApiReplacementForMethodAnalyzer(Config);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            Analyzer.RegisterAnalyzers(context);
        }
    }
}
