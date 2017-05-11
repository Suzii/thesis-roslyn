using System;

namespace SampleProjectGenerator
{
    /// <summary>
    /// Helper class for generating relative document path based on project type
    /// </summary>
    public class RelativeDocumentPaths
    {
        /// <summary>
        /// Generates relative project path based on <param name="projectType"></param>
        /// </summary>
        /// <param name="projectType">Type of project to generate type for</param>
        /// <returns>Relative file path for given project type</returns>
        public static string Get(ProjectType projectType)
        {
            switch (projectType)
            {
                case ProjectType.ConsoleApp:
                    return "ConsoleApp";
                case ProjectType.WebApp:
                    return "WebApp";
                default:
                    throw new ArgumentOutOfRangeException(nameof(projectType));
            }
        }
    }
}