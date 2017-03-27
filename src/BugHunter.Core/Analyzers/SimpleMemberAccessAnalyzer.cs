using System.Linq;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core.Analyzers
{
    public class SimpleMemberAccessAnalyzer : ISyntaxNodeAnalyzer
    {
        protected readonly ApiReplacementConfig Config;
        protected readonly IDiagnosticFormatter<MemberAccessExpressionSyntax> Formatter;
        
        public SimpleMemberAccessAnalyzer(ApiReplacementConfig config, IDiagnosticFormatter<MemberAccessExpressionSyntax> formatter)
        {
            Config = config;
            Formatter = formatter;
        }

        public void Run(SyntaxNodeAnalysisContext context)
        {
            var memberAccess = (MemberAccessExpressionSyntax)context.Node;
            if (memberAccess == null)
            {
                return;
            }

            if (!IsForbiddenUsage(context, memberAccess))
            {
                return;
            }

            var diagnostic = Formatter.CreateDiagnostic(Config.Rule, memberAccess);
            context.ReportDiagnostic(diagnostic);
        }


        protected bool IsForbiddenUsage(SyntaxNodeAnalysisContext context, MemberAccessExpressionSyntax memberAccess)
        {
            var memberName = memberAccess.Name.ToString();
            if (Config.ForbiddenMembers.All(forbiddenMember => forbiddenMember != memberName))
            {
                return false;
            }

            var actualTargetType = context.SemanticModel.GetTypeInfo(memberAccess.Expression).Type as INamedTypeSymbol;

            return actualTargetType != null && Config.ForbiddenTypes.Any(forbidenType => actualTargetType.IsDerivedFrom(forbidenType, context.Compilation));
        }
    }
}