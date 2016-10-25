using System;
using System.Collections.Immutable;
using BugHunter.Helpers.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpRequestUserHostAddressAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.RequestUserHostAddress;
        
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID, 
            title: new LocalizableResourceString(nameof(CsResources.HttpRequestUserHostAddress_Title), CsResources.ResourceManager, typeof(CsResources)),
            messageFormat: new LocalizableResourceString(nameof(CsResources.HttpRequestUserHostAddress_MessageFormat), CsResources.ResourceManager, typeof(CsResources)), 
            category: AnalyzerCategories.CsRules, 
            defaultSeverity: DiagnosticSeverity.Warning, 
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CsResources.HttpRequestUserHostAddress_Description), CsResources.ResourceManager, typeof(CsResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            RegisterAction(context, typeof(System.Web.HttpRequest), nameof(System.Web.HttpRequest.UserHostAddress));
            RegisterAction(context, typeof(System.Web.HttpRequestBase), nameof(System.Web.HttpRequestBase.UserHostAddress));
        }

        private void RegisterAction(AnalysisContext context, Type accessedType, string memberName)
        {
            var analyzer = new MemberAccessAnalysisHelper(Rule, accessedType, memberName);

            context.RegisterSyntaxNodeAction(c => analyzer.Analyze(c), SyntaxKind.SimpleMemberAccessExpression);
        }
    }
}
