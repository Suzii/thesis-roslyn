using System;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core._experiment
{
    public class ConditionalAccessAnalyzer : IAccessAnalyzer
    {
        private readonly DiagnosticDescriptor _rule;
        private readonly string _forbiddenTypeName;
        private readonly string _forbiddenMemberName;
        private readonly IDiagnosticFormatter _formatter;

        public ConditionalAccessAnalyzer(DiagnosticDescriptor rule, string forbiddenTypeName, string forbiddenMemberName) 
            : this(rule, forbiddenTypeName, forbiddenMemberName, DiagnosticFormatterFactory.CreateConditionalAccessFormatter())
        {
        }

        public ConditionalAccessAnalyzer(DiagnosticDescriptor rule, string forbiddenTypeName, string forbiddenMemberName, IDiagnosticFormatter formatter)
        {
            _rule = rule;
            _forbiddenTypeName = forbiddenTypeName;
            _forbiddenMemberName = forbiddenMemberName;
            _formatter = formatter;
        }

        public void Run(SyntaxNodeAnalysisContext context)
        {
            var conditinoalAccess = (ConditionalAccessExpressionSyntax)context.Node;
            if (conditinoalAccess == null)
            {
                return;
            }

            if (!IsForbiddenUsage(context, conditinoalAccess))
            {
                return;
            }

            var diagnostic = CreateDiagnostic(conditinoalAccess);
            context.ReportDiagnostic(diagnostic);
        }

        protected bool IsForbiddenUsage(SyntaxNodeAnalysisContext context, ConditionalAccessExpressionSyntax conditionalAccess)
        {
            var whereNotNull = conditionalAccess.WhenNotNull.ToString();
            if (!whereNotNull.StartsWith("."+_forbiddenMemberName, StringComparison.Ordinal))
            {
                return false;
            }

            var actualTargetType = context.SemanticModel.GetTypeInfo(conditionalAccess.Expression).Type as INamedTypeSymbol;

            return actualTargetType?.IsDerivedFrom(_forbiddenTypeName, context.Compilation) ?? false;
        }

        protected Diagnostic CreateDiagnostic(ConditionalAccessExpressionSyntax conditionalAccess)
        {
            var location = _formatter.GetLocation(conditionalAccess);
            var diagnosedUsage = _formatter.GetDiagnosedUsage(conditionalAccess);
            var diagnostic = Diagnostic.Create(_rule, location, diagnosedUsage);

            return diagnostic;
        }
    }
}