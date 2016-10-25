using System;
using System.Collections.Immutable;
using BugHunter.Helpers.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RequestUserHostAddressAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.RequestUserHostAddress;
        
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID, 
            title: new LocalizableResourceString(nameof(CsResources.BH1002_Title), CsResources.ResourceManager, typeof(CsResources)),
            messageFormat: new LocalizableResourceString(nameof(CsResources.BH1002_MessageFormat), CsResources.ResourceManager, typeof(CsResources)), 
            category: AnalyzerCategories.CsRules, 
            defaultSeverity: DiagnosticSeverity.Warning, 
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CsResources.BH1002_Description), CsResources.ResourceManager, typeof(CsResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            RegisterAction(context, typeof(System.Web.HttpRequest));
            RegisterAction(context, typeof(System.Web.HttpRequestBase));
        }

        private void RegisterAction(AnalysisContext context, Type accessedType)
        {
            var memberName = nameof(System.Web.HttpRequest.UserHostAddress);
            var analyzer = new MemberAccessAnalysisHelper(Rule, accessedType, memberName);

            context.RegisterSyntaxNodeAction(c => analyzer.Analyze(c), SyntaxKind.SimpleMemberAccessExpression);
        }
    }
}
