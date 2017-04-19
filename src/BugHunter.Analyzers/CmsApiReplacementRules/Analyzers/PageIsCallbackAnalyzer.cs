﻿using System.Collections.Immutable;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiReplacementRules.Analyzers
{
    /// <summary>
    /// Searches for usages of <see cref="System.Web.UI.Page"/> and their access to IsCallback member
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PageIsCallbackAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.PAGE_IS_CALLBACK;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRulesProvider.GetRule(DIAGNOSTIC_ID, "Page.IsCallback", "RequestHelper.IsCallback()");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        private static readonly ApiReplacementConfig apiReplacementConfig = new ApiReplacementConfig(Rule,
            new []{ "System.Web.UI.Page"},
            new []{ "IsCallback"});

        private static readonly ApiReplacementForMemberAnalyzer apiReplacementAnalyzer = new ApiReplacementForMemberAnalyzer(apiReplacementConfig);

        public override void Initialize(AnalysisContext context)
        {
            apiReplacementAnalyzer.RegisterAnalyzers(context);
        }
    }
}
