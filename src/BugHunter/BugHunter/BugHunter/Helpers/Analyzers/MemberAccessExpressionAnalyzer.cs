﻿using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Helpers.Analyzers
{
    internal class MemberAccessExpressionAnalyzer
    {
        private readonly DiagnosticDescriptor _rule;

        private readonly Type _accessedType;

        private readonly string[] _memberNames;

        private readonly bool _shouldUnboundGenerics;

        public MemberAccessExpressionAnalyzer(DiagnosticDescriptor rule, Type accessedType, string memberName, bool shouldUnboundGenerics = false)
            : this(rule, accessedType, new []{ memberName}, shouldUnboundGenerics) { }

        public MemberAccessExpressionAnalyzer(DiagnosticDescriptor rule, Type accessedType, string[] memberNames, bool shouldUnboundGenerics = false)
        {
            _rule = rule;
            _accessedType = accessedType;
            _memberNames = memberNames;
            _shouldUnboundGenerics = shouldUnboundGenerics;
        }

        public void Analyze(SyntaxNodeAnalysisContext context)
        {
            var memberAccess = (MemberAccessExpressionSyntax)context.Node;

            var memberName = memberAccess.Name.ToString();
            if (!_memberNames.Contains(memberName))
            {
                return;
            }

            var searchedTargetType = _accessedType.GetITypeSymbol(context);
            var actualTargetType = new SemanticModelBrowser(context).GetMemberAccessTarget(memberAccess) as INamedTypeSymbol;
            if (actualTargetType == null || !actualTargetType.IsDerivedFromClassOrInterface(searchedTargetType, _shouldUnboundGenerics))
            {
                return;
            }

            var diagnostic = Diagnostic.Create(_rule, memberAccess.GetLocation(), memberName);
            context.ReportDiagnostic(diagnostic);
        }
    }
}