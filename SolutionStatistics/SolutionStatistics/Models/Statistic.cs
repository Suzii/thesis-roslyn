namespace SolutionStatistics.Models
{
    /// <summary>
    /// Structure for grouping statistics about nodes, tokens and trivia
    /// </summary>
    internal struct Statistic
    {
        /// <summary>
        /// Detailed statistics about nodes
        /// </summary>
        public NodesStatistic NodesStatistic { get; }

        /// <summary>
        /// Number of tokens statistic
        /// </summary>
        public int NumberOfTokens { get; }

        /// <summary>
        /// Number of trivia statistic
        /// </summary>
        public int NumberOfTrivia { get; }

        /// <summary>
        /// Constructor accepting number of tokens, trivia, and detailed node statistics
        /// </summary>
        /// <param name="nodesStatistic">Detailed stats about nodes</param>
        /// <param name="numberOfTokens">Number of tokens</param>
        /// <param name="numberOfTrivia">Number of trivia</param>
        public Statistic(NodesStatistic nodesStatistic, int numberOfTokens, int numberOfTrivia)
        {
            NodesStatistic = nodesStatistic;
            NumberOfTokens = numberOfTokens;
            NumberOfTrivia = numberOfTrivia;
        }

        /// <summary>
        /// Sums two <see cref="Statistic"/> structures and returns a new one
        /// </summary>
        /// <param name="statistic1">First operand</param>
        /// <param name="statistic2">Second operand</param>
        /// <returns>New <see cref="Statistic"/> structure containing sum of two statistics</returns>
        public static Statistic operator +(Statistic statistic1, Statistic statistic2)
        {
            return new Statistic(
                statistic1.NodesStatistic + statistic2.NodesStatistic,
                statistic1.NumberOfTokens + statistic2.NumberOfTokens,
                statistic1.NumberOfTrivia + statistic2.NumberOfTrivia);
        }
    }
}
