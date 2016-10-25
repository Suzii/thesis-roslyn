using System;
using BugHunter.Helpers.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    public abstract class BaseMemberAccessAnalyzer : DiagnosticAnalyzer
    {
        protected void RegisterAction(DiagnosticDescriptor rule, AnalysisContext context, Type accessedType, params string[] memberNames)
        {
            if (memberNames.Length == 0)
            {
                return;
            }

            var analyzer = new MemberAccessAnalysisHelper(rule, accessedType, memberNames);

            context.RegisterSyntaxNodeAction(c => analyzer.Analyze(c), SyntaxKind.SimpleMemberAccessExpression);
        }
    }
}