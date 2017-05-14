// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SolutionStatistics.Models
{
    /// <summary>
    /// Structure for grouping statistics about nodes, tokens and trivia
    /// </summary>
    internal struct Statistic
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Statistic"/> struct.
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
        /// Gets detailed statistics about nodes
        /// </summary>
        public NodesStatistic NodesStatistic { get; }

        /// <summary>
        /// Gets number of tokens statistic
        /// </summary>
        public int NumberOfTokens { get; }

        /// <summary>
        /// Gets number of trivia statistic
        /// </summary>
        public int NumberOfTrivia { get; }

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
