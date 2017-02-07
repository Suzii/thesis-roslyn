using System.Collections.Immutable;
using BugHunter.Core;
using BugHunter.Core.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConnectionHelperExecuteQueryAnalyzer : BaseMemberAccessAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.CONNECTION_HELPER_EXECUTE_QUERY;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
                title: "Do not use ExecuteQuery in UI.",
                messageFormat: "'{0}()' should not be called directly from this file. Move logic to codebehind.",
                category: AnalyzerCategories.CS_RULES,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: "'ExecuteQuery()' should not be called directly from this file. Move logic to codebehind.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            RegisterAction(Rule, context, "CMS.DataEngine.ConnectionHelper", "ExecuteQuery");
        }

        protected override bool CheckPreConditions(SyntaxNodeAnalysisContext context)
        {
            return base.CheckPreConditions(context) && IsUiFile(context.Node.SyntaxTree.FilePath);
        }

        private bool IsUiFile(string filePath)
        {
            return filePath.EndsWith(ProjectPaths.Extensions.PAGES) 
                || filePath.EndsWith(ProjectPaths.Extensions.CONTROLS) 
                || filePath.EndsWith(ProjectPaths.Extensions.HANDLERS)
                || filePath.EndsWith(ProjectPaths.Extensions.MASTER_PAGE);
        }
    }
}
