// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SolutionStatistics.Models
{
    /// <summary>
    /// Structure containing detailed statistics about SyntaxNodes
    /// </summary>
    internal struct NodesStatistic
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodesStatistic"/> struct.
        /// </summary>
        /// <param name="numberOfNodesTotal">Total number of nodes</param>
        /// <param name="numberOfMemberAccessExpressionNodes">Number of <see cref="Microsoft.CodeAnalysis.CSharp.Syntax.MemberAccessExpressionSyntax"/> nodes</param>
        /// <param name="numberOfConditionalAccessExpressionNodes">Number of <see cref="Microsoft.CodeAnalysis.CSharp.Syntax.ConditionalAccessExpressionSyntax"/> nodes</param>
        /// <param name="numberOfInvocationExpressionNodes">Number of <see cref="Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax"/> nodes</param>
        /// <param name="numberOfElementAccessExpressionNodes">Number of <see cref="Microsoft.CodeAnalysis.CSharp.Syntax.ElementAccessExpressionSyntax"/> nodes</param>
        /// <param name="numberOfIdentifierNameNodes">Number of <see cref="Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax"/> nodes</param>
        /// <param name="numberOfObjectCreationNodes">Number of <see cref="Microsoft.CodeAnalysis.CSharp.Syntax.ObjectCreationExpressionSyntax"/> nodes</param>
        /// <param name="numberOfClassDeclarationNodes">Number of <see cref="Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax"/> nodes</param>
        public NodesStatistic(
            int numberOfNodesTotal,
            int numberOfMemberAccessExpressionNodes,
            int numberOfConditionalAccessExpressionNodes,
            int numberOfInvocationExpressionNodes,
            int numberOfElementAccessExpressionNodes,
            int numberOfIdentifierNameNodes,
            int numberOfObjectCreationNodes,
            int numberOfClassDeclarationNodes)
        {
            NumberofNodesTotal = numberOfNodesTotal;
            NumberOfMemberAccessExpressionNodes = numberOfMemberAccessExpressionNodes;
            NumberOfConditionalAccessExpressionNodes = numberOfConditionalAccessExpressionNodes;
            NumberOfInvocationExpressionNodes = numberOfInvocationExpressionNodes;
            NumberElementAccessExpressionNodes = numberOfElementAccessExpressionNodes;
            NumberOfIdentifierNameNodes = numberOfIdentifierNameNodes;
            NumberOfObjectCreationNodes = numberOfObjectCreationNodes;
            NumberOfClassDeclarationNodes = numberOfClassDeclarationNodes;
        }

        /// <summary>
        /// Gets total number of nodes
        /// </summary>
        public int NumberofNodesTotal { get; }

        /// <summary>
        /// Gets number of <see cref="Microsoft.CodeAnalysis.CSharp.Syntax.MemberAccessExpressionSyntax"/> nodes
        /// </summary>
        public int NumberOfMemberAccessExpressionNodes { get; }

        /// <summary>
        /// Gets number of <see cref="Microsoft.CodeAnalysis.CSharp.Syntax.ConditionalAccessExpressionSyntax"/> nodes
        /// </summary>
        public int NumberOfConditionalAccessExpressionNodes { get; }

        /// <summary>
        /// Gets number of <see cref="Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax"/> nodes
        /// </summary>
        public int NumberOfInvocationExpressionNodes { get; }

        /// <summary>
        /// Gets number of <see cref="Microsoft.CodeAnalysis.CSharp.Syntax.ElementAccessExpressionSyntax"/> nodes
        /// </summary>
        public int NumberElementAccessExpressionNodes { get; }

        /// <summary>
        /// Gets number of <see cref="Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax"/> nodes
        /// </summary>
        public int NumberOfIdentifierNameNodes { get; }

        /// <summary>
        /// Gets number of <see cref="Microsoft.CodeAnalysis.CSharp.Syntax.ObjectCreationExpressionSyntax"/> nodes
        /// </summary>
        public int NumberOfObjectCreationNodes { get; }

        /// <summary>
        /// Gets number of <see cref="Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax"/> nodes
        /// </summary>
        public int NumberOfClassDeclarationNodes { get; }

        /// <summary>
        /// Sums two <see cref="NodesStatistic"/> structures and returns a new one
        /// </summary>
        /// <param name="statistic1">First operand</param>
        /// <param name="statistic2">Second operand</param>
        /// <returns>New <see cref="NodesStatistic"/> structure containing sum of two nodes statistics</returns>
        public static NodesStatistic operator +(NodesStatistic statistic1, NodesStatistic statistic2)
        {
            return new NodesStatistic(
                statistic1.NumberofNodesTotal + statistic2.NumberofNodesTotal,
                statistic1.NumberOfMemberAccessExpressionNodes + statistic2.NumberOfMemberAccessExpressionNodes,
                statistic1.NumberOfConditionalAccessExpressionNodes + statistic2.NumberOfConditionalAccessExpressionNodes,
                statistic1.NumberOfInvocationExpressionNodes + statistic2.NumberOfInvocationExpressionNodes,
                statistic1.NumberElementAccessExpressionNodes + statistic2.NumberElementAccessExpressionNodes,
                statistic1.NumberOfIdentifierNameNodes + statistic2.NumberOfIdentifierNameNodes,
                statistic1.NumberOfObjectCreationNodes + statistic2.NumberOfObjectCreationNodes,
                statistic1.NumberOfClassDeclarationNodes + statistic2.NumberOfClassDeclarationNodes);
        }
    }
}