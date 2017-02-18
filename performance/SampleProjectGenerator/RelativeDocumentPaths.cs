using System;

namespace SampleProjectGenerator
{
    public class RelativeDocumentPaths
    {
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