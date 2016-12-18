using System.Collections.Generic;
using System.Linq;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    /// <summary>
    /// Used for analysis of SimpleMemberAccess such as "page.IsCallback"
    /// </summary>
    public abstract class BaseMemberAccessAnalyzer : DiagnosticAnalyzer
    {
        protected void RegisterAction(DiagnosticDescriptor rule, AnalysisContext context, string accessedType, string memberName, params string[] additionalMemberNames)
        {
            var forbiddenMemberNames = new HashSet<string>(additionalMemberNames) { memberName };

            context.RegisterSyntaxNodeAction(c => Analyze(rule, c, accessedType, forbiddenMemberNames), SyntaxKind.SimpleMemberAccessExpression);
        }

        protected virtual IDiagnosticFormatter GetDiagnosticFormatter()
        {
            return DiagnosticFormatterFactory.CreateMemberAccessFormatter();
        }

        private void Analyze(DiagnosticDescriptor rule, SyntaxNodeAnalysisContext context, string accessedType, ISet<string> forbiddenMemberNames)
        {
            if (!CheckPreConditions(context))
            {
                return;
            }

            if (!CheckMainConditions(context, accessedType, forbiddenMemberNames))
            {
               return;
            }

            var memberAccess = context.Node as MemberAccessExpressionSyntax;
            if (!CheckPostConditions(context, memberAccess))
            {
                return;
            }

            var diagnostic = CreateDiagnostic(rule, memberAccess);

            context.ReportDiagnostic(diagnostic);
        }

        // Can be overriden by subClasses if additional checks are needed
        protected virtual bool CheckPreConditions(SyntaxNodeAnalysisContext context)
        {
            // TODO check if file is generated
            return true;
        }

        protected virtual bool CheckMainConditions(SyntaxNodeAnalysisContext context, string accessedType, ISet<string> memberNames)
        {
            var memberAccess = (MemberAccessExpressionSyntax) context.Node;
            if (memberAccess == null)
            {
                return false;
            }

            var memberName = memberAccess.Name.ToString();
            if (!memberNames.Contains(memberName))
            {
                return false;
            }

            var searchedTargetType = context.SemanticModel.Compilation.GetTypeByMetadataName(accessedType);
            var actualTargetType = new SemanticModelBrowser(context).GetMemberAccessTarget(memberAccess) as INamedTypeSymbol;
            if (searchedTargetType == null || 
                actualTargetType == null ||
                !actualTargetType.IsDerivedFromClassOrInterface(searchedTargetType))
            {
                return false;
            }

            return true;
        }

        // To be overriden by subClasses if additional checks are needed
        protected virtual bool CheckPostConditions(SyntaxNodeAnalysisContext context, MemberAccessExpressionSyntax memberAccess)
        {
            return true;
        }

        protected virtual Diagnostic CreateDiagnostic(DiagnosticDescriptor rule, MemberAccessExpressionSyntax memberAccess)
        {
            var diagnosticFormatter = GetDiagnosticFormatter();
            var usedAs = diagnosticFormatter.GetDiagnosedUsage(memberAccess);
            var location = diagnosticFormatter.GetLocation(memberAccess);

            return Diagnostic.Create(rule, location, usedAs);
        }
    }
}