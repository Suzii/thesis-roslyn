namespace BugHunter.Core.Models
{
    /// <summary>
    /// Structure representing pair of class name and name of namespace it is defined in
    /// </summary>
    public struct ClassAndItsNamespace
    {
        /// <summary>
        /// Gets or sets name of the class
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets name of the namespace class is defined in
        /// </summary>
        public string ClassNamespace { get; set; }
    }
}