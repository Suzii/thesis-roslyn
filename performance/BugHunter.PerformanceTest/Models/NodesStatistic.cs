namespace BugHunter.PerformanceTest.Models
{
    internal struct NodesStatistic
    {
        public int NumberofNodesTotal { get; }

        public int NumberOfMemberAccessExpressionNodes { get; }

        public int NumberOfInvocationExpressionNodes { get;}

        public int NumberElementAccessExpressionNodes { get;}

        public int NumberOfIdentifierNameNodes { get;}

        public NodesStatistic(int numberOfNodexTOtal, int numberOfMemberAccessExpressionNodes, int numberOfInvocationExpressionNodes, int numberOfElementAccessExpressionNodes, int numberOfIdentifierNameNodes)
        {
            NumberofNodesTotal = numberOfNodexTOtal;
            NumberOfMemberAccessExpressionNodes = numberOfMemberAccessExpressionNodes;
            NumberOfInvocationExpressionNodes = numberOfInvocationExpressionNodes;
            NumberElementAccessExpressionNodes = numberOfElementAccessExpressionNodes;
            NumberOfIdentifierNameNodes = numberOfIdentifierNameNodes;
        }

        public static NodesStatistic operator +(NodesStatistic statistic1, NodesStatistic statistic2)
        {
            return new NodesStatistic(
                statistic1.NumberofNodesTotal + statistic2.NumberofNodesTotal,
                statistic1.NumberOfMemberAccessExpressionNodes + statistic2.NumberOfMemberAccessExpressionNodes,
                statistic1.NumberOfInvocationExpressionNodes + statistic2.NumberOfInvocationExpressionNodes,
                statistic1.NumberElementAccessExpressionNodes + statistic2.NumberElementAccessExpressionNodes,
                statistic1.NumberOfIdentifierNameNodes + statistic2.NumberOfIdentifierNameNodes);
        }
    }
}