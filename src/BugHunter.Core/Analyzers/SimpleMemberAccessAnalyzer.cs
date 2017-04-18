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
        private readonly ApiReplacementConfig _config;
        private readonly ISyntaxNodeDiagnosticFormatter<MemberAccessExpressionSyntax> _formatter;
        
        public SimpleMemberAccessAnalyzer(ApiReplacementConfig config, ISyntaxNodeDiagnosticFormatter<MemberAccessExpressionSyntax> formatter)
        {
            _config = config;
            _formatter = formatter;
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

            var diagnostic = _formatter.CreateDiagnostic(_config.Rule, memberAccess);
            context.ReportDiagnostic(diagnostic);
        }


        private bool IsForbiddenUsage(SyntaxNodeAnalysisContext context, MemberAccessExpressionSyntax memberAccess)
        {
            var memberName = memberAccess.Name.ToString();
            if (_config.ForbiddenMembers.All(forbiddenMember => forbiddenMember != memberName))
            {
                return false;
            }

            var actualTargetType = context.SemanticModel.GetTypeInfo(memberAccess.Expression).Type as INamedTypeSymbol;

            return actualTargetType != null && _config.ForbiddenTypes.Any(forbidenType => actualTargetType.IsDerivedFrom(forbidenType, context.Compilation));
        }
    }
}