using System.Collections.Generic;
using BugHunter.Core.DiagnosticsFormatting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core.Analyzers
{
    public abstract class BaseMemberAccessOrInvocationAnalyzer<TSyntaxNode> : DiagnosticAnalyzer where TSyntaxNode : SyntaxNode
    {
        protected abstract IDiagnosticFormatter<TSyntaxNode> DiagnosticFormatter { get; }

        protected void Analyze(DiagnosticDescriptor rule, SyntaxNodeAnalysisContext context, string accessedType, ISet<string> forbiddenMemberNames)
        {
            if (!CheckPreConditions(context))
            {
                return;
            }

            if (!CheckMainConditions(context, accessedType, forbiddenMemberNames))
            {
                return;
            }

            var syntaxNode = context.Node as TSyntaxNode;
            if (!CheckPostConditions(context, syntaxNode))
            {
                return;
            }

            var diagnostic = CreateDiagnostic(rule, syntaxNode);
            context.ReportDiagnostic(diagnostic);
        }

        // Can be overriden by subClasses if additional checks are needed
        protected virtual bool CheckPreConditions(SyntaxNodeAnalysisContext context)
        {
            return true;
        }

        protected abstract bool CheckMainConditions(SyntaxNodeAnalysisContext context, string accessedType, ISet<string> memberNames);

        // To be overriden by subClasses if additional checks are needed
        protected virtual bool CheckPostConditions(SyntaxNodeAnalysisContext context, TSyntaxNode syntaxNode)
        {
            return true;
        }

        protected virtual Diagnostic CreateDiagnostic(DiagnosticDescriptor rule, TSyntaxNode syntaxNode)
        {
            var usedAs = DiagnosticFormatter.GetDiagnosedUsage(syntaxNode);
            var location = DiagnosticFormatter.GetLocation(syntaxNode);

            return Diagnostic.Create(rule, location, usedAs);
        }

    }
}