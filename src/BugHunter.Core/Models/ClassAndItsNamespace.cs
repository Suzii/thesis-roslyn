namespace BugHunter.Core.Models
{
    /// <summary>
    /// Structure representing pair of class name and name of namespace it is defined in
    /// </summary>
    public struct ClassAndItsNamespace
    {
        /// <summary>
        /// Name of the class
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Name of the namespace class is defined in
        /// </summary>
        public string ClassNamespace { get; set; }
    }
}