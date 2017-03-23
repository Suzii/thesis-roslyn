using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core._experiment
{
    // TODO diagnostic formatter set by generic
    public class SimpleMemberAccessAnalyzer : IAccessAnalyzer
    {
        private readonly DiagnosticDescriptor _rule;
        private readonly string _forbiddenTypeName;
        private readonly string _forbiddenMemberName;
        private readonly IDiagnosticFormatter _formatter;

        public SimpleMemberAccessAnalyzer(DiagnosticDescriptor rule, string forbiddenTypeName, string forbiddenMemberName) 
            : this(rule, forbiddenTypeName, forbiddenMemberName, DiagnosticFormatterFactory.CreateMemberAccessFormatter())
        {
        }

        public SimpleMemberAccessAnalyzer(DiagnosticDescriptor rule, string forbiddenTypeName, string forbiddenMemberName, IDiagnosticFormatter formatter)
        {
            _rule = rule;
            _forbiddenTypeName = forbiddenTypeName;
            _forbiddenMemberName = forbiddenMemberName;
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

            var diagnostic = CreateDiagnostic(memberAccess);
            context.ReportDiagnostic(diagnostic);
        }


        protected bool IsForbiddenUsage(SyntaxNodeAnalysisContext context, MemberAccessExpressionSyntax memberAccess)
        {
            var memberName = memberAccess.Name.ToString();
            if (memberName != _forbiddenMemberName)
            {
                return false;
            }

            var actualTargetType = context.SemanticModel.GetTypeInfo(memberAccess.Expression).Type as INamedTypeSymbol;

            return actualTargetType?.IsDerivedFrom(_forbiddenTypeName, context.Compilation) ?? false;
        }

        protected Diagnostic CreateDiagnostic(MemberAccessExpressionSyntax memberAccess)
        {
            var location = _formatter.GetLocation(memberAccess);
            var diagnosedUsage = _formatter.GetDiagnosedUsage(memberAccess);
            var diagnostic = Diagnostic.Create(_rule, location, diagnosedUsage);

            return diagnostic;
        }
    }
}