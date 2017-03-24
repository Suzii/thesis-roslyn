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

        public MemberInvocationAnalyzer(ApiReplacementConfig config) 
            : this(config, DiagnosticFormatterFactory.CreateMemberInvocationFormatter())
        {
        }

        private MemberInvocationAnalyzer(ApiReplacementConfig config, IDiagnosticFormatter<InvocationExpressionSyntax> formatter)
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

            var invocation = (InvocationExpressionSyntax)context.Node;
            if (invocation == null)
            {
                return;
            }

            IMethodSymbol methodSymbol;
            if (!IsForbiddenMethodOnForbiddenType(context, invocation, out methodSymbol))
            {
                return;
            }

            if (!IsForbiddenUsage(methodSymbol))
            {
                return;
            }

            var diagnostic = CreateDiagnostic(invocation);
            context.ReportDiagnostic(diagnostic);
        }

        protected bool IsOnForbiddenPath(string filePath)
            => true;

        protected bool IsForbiddenMethodOnForbiddenType(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, out IMethodSymbol methodSymbol)
        {
            // TODO analyze based on syntax first
            // check for simpleMemberAccess as child or find MemberBindingExpression.. 
            // if child is IdentifierName, it is not a member invocation but a direct one
            
            var semanticModel = context.SemanticModel;
            var invokedMethodSymbol = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            methodSymbol = invokedMethodSymbol;
            if (invokedMethodSymbol == null)
            {
                return false;
            }

            if (Config.ForbiddenMembers.All(forbiddenMember => forbiddenMember != invokedMethodSymbol.Name))
            {
                return false;
            }

            var receiverTypeSymbol = invokedMethodSymbol.ReceiverType as INamedTypeSymbol;
            if (receiverTypeSymbol == null || 
                Config.ForbiddenTypes.All(forbiddenType => !receiverTypeSymbol.IsDerivedFrom(forbiddenType, context.Compilation)))
            {
                return false;
            }

            return true;
        }

        protected bool IsForbiddenUsage(IMethodSymbol methodSymbol)
            => true;

        protected Diagnostic CreateDiagnostic(InvocationExpressionSyntax invocation)
        {
            var location = Formatter.GetLocation(invocation);
            var diagnosedUsage = Formatter.GetDiagnosedUsage(invocation);
            var diagnostic = Diagnostic.Create(Config.Rule, location, diagnosedUsage);

            return diagnostic;
        }
    }
}