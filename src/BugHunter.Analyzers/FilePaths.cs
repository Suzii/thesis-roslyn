namespace BugHunter.Analyzers
{
    public static class FilePaths
    {
        public static class Folders
        {
            /// <summary>
            /// This folder contains webparts for UI elements
            /// </summary>
            public const string UI_WEB_PARTS = @"CMSModules\AdminControls\Controls\UIControls\";

            /// <summary>
            /// This folder contains webparts
            /// </summary>
            public const string WEB_PARTS = @"CMSWebParts\";
        }

        public static class Extensions
        {
            public const string PAGES = ".aspx.cs";
            public const string CONTROLS = ".ascx.cs";
            public const string HANDLERS = ".ashx.cs";
            public const string MASTER_PAGE = ".master.cs";
            public const string DESIGNER_FILES = ".designer.cs";
        }
    }
}