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
    /// <summary>
    /// Analyzing strategy for <see cref="MemberAccessExpressionSyntax"/>
    ///
    /// Runs the analysis for current context based on the <see cref="ApiReplacementConfig"/>
    /// and raises diagnostics using passed <see cref="ISyntaxNodeDiagnosticFormatter{TSyntaxNode}"/>
    /// </summary>
    public class SimpleMemberAccessAnalyzer : ISyntaxNodeAnalyzer
    {
        private readonly ApiReplacementConfig _config;
        private readonly ISyntaxNodeDiagnosticFormatter<MemberAccessExpressionSyntax> _formatter;

        /// <summary>
        /// Constructor accepting <param name="config"></param> and <param name="formatter"></param>
        /// </summary>
        /// <param name="config">Configuration to be used for the analysis, specifying forbidden members on specific types and diagnostic descriptor</param>
        /// <param name="formatter">Diagnostic formatter to be used for creation of diagnostic</param>
        public SimpleMemberAccessAnalyzer(ApiReplacementConfig config, ISyntaxNodeDiagnosticFormatter<MemberAccessExpressionSyntax> formatter)
        {
            _config = config;
            _formatter = formatter;
        }

        /// <summary>
        /// Runs the analysis for current <paramref name="context"/> and raises <see cref="Diagnostic"/> if usage is qualified as forbidden
        /// </summary>
        /// <param name="context">Context to perform analysis on</param>
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