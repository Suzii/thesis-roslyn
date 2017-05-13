// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace BugHunter.TestUtils
{
    public class ReferencesHelper
    {
        // System
        public static readonly MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        public static readonly MetadataReference SystemReference = MetadataReference.CreateFromFile(typeof(Uri).Assembly.Location);
        public static readonly MetadataReference SystemCoreReference = MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);
        public static readonly MetadataReference CSharpSymbolsReference = MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
        public static readonly MetadataReference CodeAnalysisReference = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);

        public static readonly MetadataReference[] CoreDotNetReferences =
        {
            CorlibReference,
            SystemReference,
            SystemCoreReference,
            CSharpSymbolsReference,
            CodeAnalysisReference
        };

        // System.Web
        public static readonly MetadataReference SystemWebReference = MetadataReference.CreateFromFile(typeof(System.Web.HttpRequest).Assembly.Location);
        public static readonly MetadataReference SystemWebUIReference = MetadataReference.CreateFromFile(typeof(System.Web.UI.Control).Assembly.Location);

        // Kentico.Libraries
        public static readonly MetadataReference[] CMSBasicReferences = GetReferencesFor(
            typeof(CMS.Core.ModuleName),
            typeof(CMS.Base.BaseModule),
            typeof(CMS.DataEngine.TypeCondition),
            typeof(CMS.Helpers.AJAXHelper),
            typeof(CMS.IO.AbstractFile),
            typeof(CMS.EventLog.EventType));

        // Kentico.Librarie.Web.Ui
        public static readonly MetadataReference CMSBaseWebUI = MetadataReference.CreateFromFile(typeof(CMS.Base.Web.UI.ScriptHelper).Assembly.Location);
        public static readonly MetadataReference CMSUiControls = MetadataReference.CreateFromFile(typeof(CMS.UIControls.CMSAbstractUIWebpart).Assembly.Location);

        public static MetadataReference[] GetReferencesFor(params Type[] types)
        {
            if (types == null || types.Length == 0)
            {
                return new MetadataReference[] { };
            }

            var references = types.Select(type => MetadataReference.CreateFromFile(type.Assembly.Location)).Distinct();

            return references.ToArray();
        }
    }
}
