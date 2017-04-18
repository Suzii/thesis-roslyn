using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SolutionStatistics.Models;

namespace SolutionStatistics
{
    /// <summary>
    /// Helper class for <see cref="Statistic"/> creation and aggregation
    /// </summary>
    internal class StatisticsHelper
    {
        /// <summary>
        /// Determines whether a given <param name="project"></param> should be excluded from stats gathering
        /// 
        /// If the project is one of third-party projects or a test project, it does not have analyzers installed, and therefore shall be excluded from statistics
        /// </summary>
        /// <param name="project">Project to be inspected</param>
        /// <returns>True if the project should not be considered fro stats gathering</returns>
        public static bool IsProjectExcluded(Project project)
        {
            var thirdPartyProjects = new[]
            {
                "Contrib.WordNet.SynExpand",
                "FiftyOne",
                "ITHitWebDAVServer",
                "Lucene.Net.v3",
                "PDFClown",
                "QRCodeLib",
            };

            Console.WriteLine(project.FilePath);
            return project.FilePath.Contains("Tests") || thirdPartyProjects.Any(thirdPartyProject => project.FilePath.Contains(thirdPartyProject));
        }

        /// <summary>
        /// Prints given <param name="statistics"></param> to Console
        /// </summary>
        /// <param name="statistics">Statistics to be printed outs</param>
        public static void PrintStatistics(Statistic statistics)
        {
            Console.WriteLine("Number of syntax tokens: \t\t{0,12:N}", statistics.NumberOfTokens);
            Console.WriteLine("Number of syntax trivia: \t\t{0,12:N}", statistics.NumberOfTrivia);
            Console.WriteLine("Number of syntax nodes:\t\t\t{0,12:N}", statistics.NodesStatistic.NumberofNodesTotal);
            Console.WriteLine("- identifierName:      \t\t\t{0,12:N}", statistics.NodesStatistic.NumberOfIdentifierNameNodes);
            Console.WriteLine("- classDeclaration:    \t\t\t{0,12:N}", statistics.NodesStatistic.NumberOfClassDeclarationNodes);
            Console.WriteLine("- objectCreation:      \t\t\t{0,12:N}", statistics.NodesStatistic.NumberOfObjectCreationNodes);
            Console.WriteLine("- memberAccess:        \t\t\t{0,12:N}", statistics.NodesStatistic.NumberOfMemberAccessExpressionNodes);
            Console.WriteLine("- conditionalAccess:        \t\t\t{0,12:N}", statistics.NodesStatistic.NumberOfConditionalAccessExpressionNodes);
            Console.WriteLine("- invocationExpression:\t\t\t{0,12:N}", statistics.NodesStatistic.NumberOfInvocationExpressionNodes);
            Console.WriteLine("- elementAccess:       \t\t\t{0,12:N}", statistics.NodesStatistic.NumberElementAccessExpressionNodes);
        }

        /// <summary>
        /// Given multiple <param name="projects"></param> calculates statistics for every one of them and aggregates them into one <see cref="Statistic"/>
        /// </summary>
        /// <param name="projects">Projects to gather stats about</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Aggregated statistics about projects</returns>
        public static Task<Statistic> GetProjectsStatisticsAsync(IEnumerable<Project> projects, CancellationToken cancellationToken)
        {
            var sums = new ConcurrentBag<Statistic>();

            Parallel.ForEach(projects.SelectMany(i => i.Documents), document =>
            {
                var documentStatistics = GetAnalyzerStatisticsAsync(document, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
                sums.Add(documentStatistics);
            });

            var sum = sums.Aggregate(new Statistic(new NodesStatistic(0, 0, 0, 0, 0, 0, 0, 0), 0, 0), (currentResult, value) => currentResult + value);
            return Task.FromResult(sum);
        }

        /// <summary>
        /// Given <param name="document"></param> returns <see cref="Statistic"/> about it
        /// </summary>
        /// <param name="document">Document to return stats about</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Statistics about given document</returns>
        private static async Task<Statistic> GetAnalyzerStatisticsAsync(Document document, CancellationToken cancellationToken)
        {
            var tree = await document.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);
            var root = await tree.GetRootAsync(cancellationToken).ConfigureAwait(false);
            var numberOfTokens = root.DescendantTokens(descendIntoTrivia: true).Count();
            var numberOfTrivia = root.DescendantTrivia(descendIntoTrivia: true).Count();
            var nodes = root.DescendantNodesAndSelf(descendIntoTrivia: true).ToArray();

            var numberOfNodes = nodes.Length;
            var numberOfMemberAccesses = nodes.Count(n => n.IsKind(SyntaxKind.SimpleMemberAccessExpression));
            var numberOfConditionalAccesses = nodes.Count(n => n.IsKind(SyntaxKind.ConditionalAccessExpression));
            var numberOfInvocations = nodes.Count(n => n.IsKind(SyntaxKind.InvocationExpression));
            var numberOfElementAccesses = nodes.Count(n => n.IsKind(SyntaxKind.ElementAccessExpression));
            var numberOfIdentifierNames = nodes.Count(n => n.IsKind(SyntaxKind.IdentifierName));
            var numberOfObjectCreations = nodes.Count(n => n.IsKind(SyntaxKind.ObjectCreationExpression));
            var numberOfClassDeclarations = nodes.Count(n => n.IsKind(SyntaxKind.ClassDeclaration));

            var nodesStats = new NodesStatistic(
                numberOfNodes,
                numberOfMemberAccesses,
                numberOfConditionalAccesses,
                numberOfInvocations,
                numberOfElementAccesses,
                numberOfIdentifierNames,
                numberOfObjectCreations,
                numberOfClassDeclarations);

            return new Statistic(nodesStats, numberOfTokens, numberOfTrivia);
        }
    }
}