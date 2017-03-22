using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core._experiment
{
    public class SimpleMemberAccessAnalyzer : IAccessAnalyzer
    {
        private readonly string _forbiddenTypeName;
        private readonly string _forbiddenMemberName;
        private readonly IDiagnosticFormatter _formatter;

        public SimpleMemberAccessAnalyzer(string forbiddenTypeName, string forbiddenMemberName) 
            : this(forbiddenTypeName, forbiddenMemberName, DiagnosticFormatterFactory.CreateMemberAccessFormatter())
        {
        }

        public SimpleMemberAccessAnalyzer(string forbiddenTypeName, string forbiddenMemberName, IDiagnosticFormatter formatter)
        {
            _forbiddenTypeName = forbiddenTypeName;
            _forbiddenMemberName = forbiddenMemberName;
            _formatter = formatter;
        }

        public bool IsForbiddenUsage(SyntaxNodeAnalysisContext context)
        {
            var memberAccess = context.Node as MemberAccessExpressionSyntax;
            if (memberAccess == null)
            {
                return false;
            }

            var memberName = memberAccess.Name.ToString();
            if (memberName != _forbiddenMemberName)
            {
                return false;
            }

            var actualTargetType = context.SemanticModel.GetTypeInfo(memberAccess.Expression).Type as INamedTypeSymbol;

            return actualTargetType?.IsDerivedFrom(_forbiddenTypeName, context.Compilation) ?? false;
        }

        public void ReportDiagnostic(SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor)
        {
            var location = _formatter.GetLocation(context.Node);
            var diagnosedUsage = _formatter.GetDiagnosedUsage(context.Node);
            var diagnostic = Diagnostic.Create(descriptor, location, diagnosedUsage);

            context.ReportDiagnostic(diagnostic);
        }
    }
}