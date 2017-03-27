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
    public class MemberInvocationAnalyzer : ISyntaxNodeAnalyzer
    {
        protected readonly ApiReplacementConfig Config;
        protected readonly IDiagnosticFormatter<InvocationExpressionSyntax> Formatter;
        
        public MemberInvocationAnalyzer(ApiReplacementConfig config, IDiagnosticFormatter<InvocationExpressionSyntax> formatter)
        {
            Config = config;
            Formatter = formatter;
        }

        public void Run(SyntaxNodeAnalysisContext context)
        {
            var filePath = context.Node?.SyntaxTree?.FilePath;
            if (!IsOnForbiddenPath(filePath))
            {
                return;
            }

            var invocation = (InvocationExpressionSyntax) context.Node;
            if (invocation == null || CanBeSkippedBasedOnSyntaxOnly(invocation))
            {
                return;
            }

            var invokedMethodSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            if (invokedMethodSymbol == null || !IsForbiddenMethodOnForbiddenType(invocation, invokedMethodSymbol, context.Compilation))
            {
                return;
            }

            if (!IsForbiddenUsage(invocation, invokedMethodSymbol))
            {
                return;
            }

            var diagnostic = CreateDiagnostic(invocation);
            context.ReportDiagnostic(diagnostic);
        }

        protected virtual bool IsOnForbiddenPath(string filePath) => true;

        protected virtual bool IsForbiddenUsage(InvocationExpressionSyntax invocation, IMethodSymbol methodSymbol) => true;

        protected virtual Diagnostic CreateDiagnostic(InvocationExpressionSyntax invocation)
        {
            var location = Formatter.GetLocation(invocation);
            var diagnosedUsage = Formatter.GetDiagnosedUsage(invocation);
            var diagnostic = Diagnostic.Create(Config.Rule, location, diagnosedUsage);

            return diagnostic;
        }

        private bool CanBeSkippedBasedOnSyntaxOnly(InvocationExpressionSyntax invocationExpression)
        {
            SimpleNameSyntax methodNameNode;
            if (!invocationExpression.TryGetMethodNameNode(out methodNameNode))
            {
                return false;
            }

            return !IsForbiddenMethod(methodNameNode.Identifier.ValueText);
        }

        private bool IsForbiddenMethodOnForbiddenType(InvocationExpressionSyntax invocation, IMethodSymbol invokedMethodSymbol, Compilation compilation)
        {
            if (!IsForbiddenMethod(invokedMethodSymbol.Name))
            {
                return false;
            }

            var receiverTypeSymbol = invokedMethodSymbol.ReceiverType as INamedTypeSymbol;
            return receiverTypeSymbol != null && IsForbiddenType(receiverTypeSymbol, compilation);
        }

        private bool IsForbiddenMethod(string methodName)
            => Config.ForbiddenMembers.Contains(methodName);

        private bool IsForbiddenType(INamedTypeSymbol receiverTypeSymbol, Compilation compilation)
            => Config.ForbiddenTypes.Any(forbiddenType => receiverTypeSymbol.IsDerivedFrom(forbiddenType, compilation));
    }
}