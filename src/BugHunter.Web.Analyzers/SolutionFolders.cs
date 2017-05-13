using System;
using BugHunter.Core.Extensions;

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
        public const string UIWebParts = @"CMSModules\AdminControls\Controls\UIControls\";

        /// <summary>
        /// Relative path for folder containing webparts
        /// </summary>
        public const string WebParts = @"CMSWebParts\";

        /// <summary>
        /// Determines whether given <paramref name="filePath" /> is located in WebParts folder
        /// </summary>
        /// <param name="filePath">File path to be checked</param>
        /// <returns>True if file is in one of WebParts folders</returns>
        public static bool FileIsInWebPartsFolder(string filePath)
        {
            return !string.IsNullOrEmpty(filePath) &&
                   !filePath.Contains("_files\\") &&
                   (filePath.Contains(UIWebParts, StringComparison.OrdinalIgnoreCase) || filePath.Contains(WebParts, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks if the given file is UI web part.
        /// </summary>
        /// <param name="path">Path to web part file.</param>
        /// <returns>True if based on path the web part is a UI web part; false otherwise</returns>
        public static bool IsUIWebPart(string path)
        {
            return path?.Contains(UIWebParts, StringComparison.OrdinalIgnoreCase) ?? false;
        }
    }
}