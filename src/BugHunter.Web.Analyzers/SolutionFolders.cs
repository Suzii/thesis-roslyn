using BugHunter.Core.Extensions;
using System;

namespace BugHunter.Web.Analyzers
{
    /// <summary>
    /// Helper class containing constants and utility  functions for relative paths of folders in CMS solution
    /// </summary>
    public static class SolutionFolders
    {
        /// <summary>
        /// Relative path to folder containing webparts for UI elements
        /// </summary>
        public const string UI_WEB_PARTS = @"CMSModules\AdminControls\Controls\UIControls\";

        /// <summary>
        /// Relative path for folder containing webparts
        /// </summary>
        public const string WEB_PARTS = @"CMSWebParts\";

        /// <summary>
        /// Determines whether given <param name="filePath"></param> is located in WebParts folder
        /// </summary>
        /// <param name="filePath">File path to be checked</param>
        /// <returns>True if file is in one of WebParts folders</returns>
        public static bool FileIsInWebPartsFolder(string filePath)
        {
            return !string.IsNullOrEmpty(filePath) &&
                   !filePath.Contains("_files\\") &&
                   (filePath.Contains(UI_WEB_PARTS, StringComparison.OrdinalIgnoreCase) || filePath.Contains(WEB_PARTS, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks if the given file is UI web part.
        /// </summary>
        /// <param name="path">Path to web part file.</param>
        public static bool IsUIWebPart(string path)
        {
            return path?.Contains(UI_WEB_PARTS, StringComparison.OrdinalIgnoreCase) ?? false;
        }
    }
}