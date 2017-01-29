// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace BugHunter.PerformanceTest.Models
{
    internal struct Statistic
    {
        public NodesStatistic NodesStatistic { get; }

        public int NumberOfTokens { get; }

        public int NumberOfTrivia { get; }

        public Statistic(NodesStatistic nodesStatistic, int numberOfTokens, int numberOfTrivia)
        {
            NodesStatistic = nodesStatistic;
            NumberOfTokens = numberOfTokens;
            NumberOfTrivia = numberOfTrivia;
        }

        public static Statistic operator +(Statistic statistic1, Statistic statistic2)
        {
            return new Statistic(
                statistic1.NodesStatistic + statistic2.NodesStatistic,
                statistic1.NumberOfTokens + statistic2.NumberOfTokens,
                statistic1.NumberOfTrivia + statistic2.NumberOfTrivia);
        }
    }
}
