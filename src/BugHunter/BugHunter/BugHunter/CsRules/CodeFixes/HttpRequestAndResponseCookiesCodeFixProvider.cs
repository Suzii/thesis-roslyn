using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.CsRules.Analyzers;
using BugHunter.Helpers;
using BugHunter.Helpers.CodeFixes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.CsRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(HttpRequestAndResponseCookiesCodeFixProvider)), Shared]
    public class HttpRequestAndResponseCookiesCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(HttpRequestUserHostAddressAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var editor = new MemberAccessCodeFixHelper(context);
            var memberAccess = await editor.GetClosestMemberAccess();

            if (memberAccess == null)
            {
                return;
            }

            var codeFixTitle = new LocalizableResourceString(nameof(CsResources.ApiReplacements_CodeFix), CsResources.ResourceManager, typeof(CsResources)).ToString();
            var usingNamespace = typeof(CMS.Helpers.CookieHelper).Namespace;
            var newMemberAccess = await GetNewMemberAccess(context, memberAccess);
            var diagnostic = context.Diagnostics.First();
            
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: string.Format(codeFixTitle, newMemberAccess),
                    createChangedDocument: c => editor.ReplaceExpressionWith(memberAccess, newMemberAccess, usingNamespace),
                    equivalenceKey: nameof(HttpRequestAndResponseCookiesCodeFixProvider)),
                diagnostic);
        }

        private static async Task<ExpressionSyntax> GetNewMemberAccess(CodeFixContext context, MemberAccessExpressionSyntax oldMemberAccess)
        {
            // TODO this is sooooo shitty design --> split to separate analyzers + codefixes for request and response????
            var semanticModel = await context.Document.GetSemanticModelAsync();
            var semanticModelBrowser = new SemanticModelBrowser(semanticModel);

            var memberAccessTarget = semanticModelBrowser.GetMemberAccessTarget(oldMemberAccess) as INamedTypeSymbol;
            
            if (IsRequest(memberAccessTarget, semanticModel))
            {
                return SyntaxFactory.ParseExpression("CookieHelper.RequestCookies");
            }

            if (IsResponse(memberAccessTarget, semanticModel))
            {
                return SyntaxFactory.ParseExpression("CookieHelper.ResponseCookies");
            }

            throw new ArgumentException(nameof(context));
        }

        private static bool IsResponse(INamedTypeSymbol memberAccessTarget, SemanticModel semanticModel)
        {
            var httpResponseType = typeof(System.Web.HttpResponse).GetITypeSymbol(semanticModel.Compilation);
            var httpResponseBaseType = typeof(System.Web.HttpResponseBase).GetITypeSymbol(semanticModel.Compilation);

            var isResponse = memberAccessTarget.IsDerivedFromClassOrInterface(httpResponseType) ||
                             memberAccessTarget.IsDerivedFromClassOrInterface(httpResponseBaseType);

            return isResponse;
        }

        private static bool IsRequest(INamedTypeSymbol memberAccessTarget, SemanticModel semanticModel)
        {
            var httpRequestType = typeof(System.Web.HttpRequest).GetITypeSymbol(semanticModel.Compilation);
            var httpRequestBaseType = typeof(System.Web.HttpRequestBase).GetITypeSymbol(semanticModel.Compilation);

            var isRequest = memberAccessTarget.IsDerivedFromClassOrInterface(httpRequestType) ||
                            memberAccessTarget.IsDerivedFromClassOrInterface(httpRequestBaseType);

            return isRequest;
        }
    }
}