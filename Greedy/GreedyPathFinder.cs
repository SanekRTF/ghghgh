using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;
using NUnit.Framework;

namespace Greedy
{
    public class GreedyPathFinder : IPathFinder
    {
        public List<Point> FindPathToCompleteGoal(State gameState)
        {
            var completePath = new List<Point>();
            var visitedChests = new HashSet<Point>();
            var dijkstraPathFinder = new DijkstraPathFinder();

            for (var chestIndex = 0; chestIndex < gameState.Goal; chestIndex++)
            {
                var currentPath = dijkstraPathFinder
                    .GetPathsByDijkstra(gameState, gameState.Position, gameState.Chests
                        .Where(chest => !visitedChests.Contains(chest)))
                    .FirstOrDefault();

                if (currentPath == null || currentPath.Cost > gameState.Energy)
                    return new List<Point>();

                visitedChests.Add(currentPath.End);
                gameState.Energy -= currentPath.Cost;
                gameState.Position = currentPath.End;
                completePath.AddRange(currentPath.Path.Skip(1));
            }
            return completePath;
        }
    }
}
