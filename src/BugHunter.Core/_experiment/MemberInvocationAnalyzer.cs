using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core._experiment
{
    public class MemberInvocationAnalyzer : IAccessAnalyzer
    {
        private readonly DiagnosticDescriptor _rule;
        private readonly string _forbiddenTypeName;
        private readonly string _forbiddenMemberName;
        private readonly IDiagnosticFormatter<InvocationExpressionSyntax> _formatter;

        public MemberInvocationAnalyzer(DiagnosticDescriptor rule, string forbiddenTypeName, string forbiddenMemberName) 
            : this(rule, forbiddenTypeName, forbiddenMemberName, DiagnosticFormatterFactory.CreateMemberInvocationFormatter())
        {
        }

        public MemberInvocationAnalyzer(DiagnosticDescriptor rule, string forbiddenTypeName, string forbiddenMemberName, IDiagnosticFormatter<InvocationExpressionSyntax> formatter)
        {
            _rule = rule;
            _forbiddenTypeName = forbiddenTypeName;
            _forbiddenMemberName = forbiddenMemberName;
            _formatter = formatter;
        }

        public void Run(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            if (invocation == null)
            {
                return;
            }

            IMethodSymbol methodSymbol;
            if (!IsForbiddenUsage(context, invocation, out methodSymbol))
            {
                return;
            }

            var diagnostic = CreateDiagnostic(invocation);
            context.ReportDiagnostic(diagnostic);
        }


        protected bool IsForbiddenUsage(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, out IMethodSymbol methodSymbol)
        {
            // TODO analyze based on syntax first
            
            // rest...
            var semanticModel = context.SemanticModel;
            methodSymbol = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            if (methodSymbol == null)
            {
                return false;
            }

            if (methodSymbol.Name != _forbiddenMemberName)
            {
                return false;
            }

            var forbiddenReceiverType = semanticModel.Compilation.GetTypeByMetadataName(_forbiddenTypeName);
            var recreiverTypeSymbol = methodSymbol.ReceiverType as INamedTypeSymbol;
            if (recreiverTypeSymbol == null || forbiddenReceiverType == null || !forbiddenReceiverType.IsDerivedFrom(recreiverTypeSymbol))
            {
                return false;
            }

            return true;
        }

        protected Diagnostic CreateDiagnostic(InvocationExpressionSyntax invocation)
        {
            var location = _formatter.GetLocation(invocation);
            var diagnosedUsage = _formatter.GetDiagnosedUsage(invocation);
            var diagnostic = Diagnostic.Create(_rule, location, diagnosedUsage);

            return diagnostic;
        }
    }
}