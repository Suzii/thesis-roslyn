using Microsoft.CodeAnalysis;

namespace BugHunter.Test.CsTests
{
    public static class Constants
    {
        public static readonly MetadataReference CMSCoreReference = MetadataReference.CreateFromFile(typeof(CMS.Core.ModuleName).Assembly.Location);
        public static readonly MetadataReference CMSBaseReference = MetadataReference.CreateFromFile(typeof(CMS.Base.BaseModule).Assembly.Location);
        public static readonly MetadataReference CMSDataEngineReference = MetadataReference.CreateFromFile(typeof(CMS.DataEngine.TypeCondition).Assembly.Location);
        public static readonly MetadataReference CMSHelpersReference = MetadataReference.CreateFromFile(typeof(CMS.Helpers.AJAXHelper).Assembly.Location);
        public static readonly MetadataReference CMSIOReference = MetadataReference.CreateFromFile(typeof(CMS.IO.AbstractFile).Assembly.Location);
        public static readonly MetadataReference CMSEventLogReference = MetadataReference.CreateFromFile(typeof(CMS.EventLog.EventType).Assembly.Location);

        public static readonly MetadataReference SystemWebReference = MetadataReference.CreateFromFile(typeof(System.Web.HttpRequest).Assembly.Location);

        public static readonly MetadataReference[] BasicReferences =
        {
            CMSCoreReference,
            CMSBaseReference,
            CMSDataEngineReference,
            CMSHelpersReference,
            CMSIOReference,
            CMSEventLogReference
        };
    }
}
