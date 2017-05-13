// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.Helpers.CodeFixes
{
    /// <summary>
    /// Class handling common tasks related to code fixes of <see cref="ClassDeclarationSyntax"/>. Always instantiated with concrete <see cref="CodeFixContext"/> instance.
    /// </summary>
    public class ClassDeclarationCodeFixHelper : CodeFixHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassDeclarationCodeFixHelper"/> class.
        /// </summary>
        /// <param name="context">Context of the code fix</param>
        public ClassDeclarationCodeFixHelper(CodeFixContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Returns the <see cref="ClassDeclarationSyntax"/> associated with current code fix context at location of passed <paramref name="diagnostic" />
        /// </summary>
        /// <param name="diagnostic">Diagnostic of class declaration syntax</param>
        /// <returns>Class declarations syntax at diagnostic location</returns>
        public async Task<ClassDeclarationSyntax> GetDiagnosedClassDeclarationSyntax(Diagnostic diagnostic)
        {
            var root = await GetDocumentRoot();
            var classDeclaration = root
                .FindNode(diagnostic.Location.SourceSpan)
                .AncestorsAndSelf()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault();

            return classDeclaration;
        }
    }
}