using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BugHunter.PerformanceTest.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace BugHunter.PerformanceTest
{
    internal class StatisticsHelper
    {
        public static void PrintStatistics(Statistic statistics)
        {
            Console.WriteLine("Number of syntax tokens: \t\t{0,12:N}", statistics.NumberOfTokens);
            Console.WriteLine("Number of syntax trivia: \t\t{0,12:N}", statistics.NumberOfTrivia);
            Console.WriteLine("Number of syntax nodes:\t\t\t{0,12:N}", statistics.NodesStatistic.NumberofNodesTotal);
            Console.WriteLine("- identifierName:      \t\t\t{0,12:N}", statistics.NodesStatistic.NumberOfIdentifierNameNodes);
            Console.WriteLine("- classDeclaration:    \t\t\t{0,12:N}", statistics.NodesStatistic.NumberOfClassDeclarationNodes);
            Console.WriteLine("- objectCreation:      \t\t\t{0,12:N}", statistics.NodesStatistic.NumberOfObjectCreationNodes);
            Console.WriteLine("- memberAccess:        \t\t\t{0,12:N}", statistics.NodesStatistic.NumberOfMemberAccessExpressionNodes);
            Console.WriteLine("- invocationExpression:\t\t\t{0,12:N}", statistics.NodesStatistic.NumberOfInvocationExpressionNodes);
            Console.WriteLine("- elementAccess:       \t\t\t{0,12:N}", statistics.NodesStatistic.NumberElementAccessExpressionNodes);
        }

        public static Task<Statistic> GetAnalyzerStatisticsAsync(IEnumerable<Project> projects, CancellationToken cancellationToken)
        {
            var sums = new ConcurrentBag<Statistic>();

            Parallel.ForEach(projects.SelectMany(i => i.Documents), document =>
            {
                var documentStatistics = GetAnalyzerStatisticsAsync(document, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
                sums.Add(documentStatistics);
            });

            var sum = sums.Aggregate(new Statistic(new NodesStatistic(0, 0, 0, 0, 0, 0, 0), 0, 0), (currentResult, value) => currentResult + value);
            return Task.FromResult(sum);
        }

        public static async Task<Statistic> GetAnalyzerStatisticsAsync(Document document, CancellationToken cancellationToken)
        {
            var tree = await document.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);
            var root = await tree.GetRootAsync(cancellationToken).ConfigureAwait(false);
            var numberOfTokens = root.DescendantTokens(descendIntoTrivia: true).Count();
            var numberOfTrivia = root.DescendantTrivia(descendIntoTrivia: true).Count();
            var nodes = root.DescendantNodesAndSelf(descendIntoTrivia: true).ToArray();

            var numberOfNodes = nodes.Length;
            var numberOfMemberAccesses = nodes.Count(n => n.IsKind(SyntaxKind.SimpleMemberAccessExpression));
            var numberOfInvocations = nodes.Count(n => n.IsKind(SyntaxKind.InvocationExpression));
            var numberOfElementAccesses = nodes.Count(n => n.IsKind(SyntaxKind.ElementAccessExpression));
            var numberOfIdentifierNames = nodes.Count(n => n.IsKind(SyntaxKind.IdentifierName));
            var numberOfObjectCreations = nodes.Count(n => n.IsKind(SyntaxKind.ObjectCreationExpression));
            var numberOfClassDeclarations = nodes.Count(n => n.IsKind(SyntaxKind.ClassDeclaration));

            var nodesStats = new NodesStatistic(
                numberOfNodes,
                numberOfMemberAccesses,
                numberOfInvocations,
                numberOfElementAccesses,
                numberOfIdentifierNames,
                numberOfObjectCreations,
                numberOfClassDeclarations);

            return new Statistic(nodesStats, numberOfTokens, numberOfTrivia);
        }
    }
}