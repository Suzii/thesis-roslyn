// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace BugHunter.PerformanceTest
{
    internal struct Statistic
    {
        public int NumberofNodes { get; }

        public int NumberOfTokens { get; }

        public int NumberOfTrivia { get; }

        public Statistic(int numberOfNodes, int numberOfTokens, int numberOfTrivia)
        {
            NumberofNodes = numberOfNodes;
            NumberOfTokens = numberOfTokens;
            NumberOfTrivia = numberOfTrivia;
        }

        public static Statistic operator +(Statistic statistic1, Statistic statistic2)
        {
            return new Statistic(
                statistic1.NumberofNodes + statistic2.NumberofNodes,
                statistic1.NumberOfTokens + statistic2.NumberOfTokens,
                statistic1.NumberOfTrivia + statistic2.NumberOfTrivia);
        }
    }
}
