using System;
using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy
{
    public class NotGreedyPathFinder : IPathFinder
    {
        public List<Point> FindPathToCompleteGoal(State gameState)
        {
            var pathsDictionary = CalculatePaths(gameState);
            return GetOptimalPath(gameState, pathsDictionary);
        }

        private static Dictionary<Point, Dictionary<Point, PathWithCost>> CalculatePaths(State gameState)
        {
            var relevantPoints = new List<Point>(gameState.Chests);
            relevantPoints.Add(gameState.Position);
            var dijkstraPathFinder = new DijkstraPathFinder();

            var pathsDictionary = new Dictionary<Point, Dictionary<Point, PathWithCost>>();
            foreach (var point in relevantPoints)
            {
                pathsDictionary[point] = new Dictionary<Point, PathWithCost>();
                foreach (var path in dijkstraPathFinder.GetPathsByDijkstra(gameState, point, relevantPoints))
                {
                    pathsDictionary[point][path.End] = path;
                }
            }
            return pathsDictionary;
        }

        private static Queue<PathWithCost> InitializePathQueue(State gameState, Dictionary<Point, Dictionary<Point, PathWithCost>> paths)
        {
            var pathQueue = new Queue<PathWithCost>();
            foreach (var chest in gameState.Chests)
            {
                pathQueue.Enqueue(paths[gameState.Position][chest]);
            }
            return pathQueue;
        }

        private static List<Point> GetOptimalPath(State gameState, Dictionary<Point, Dictionary<Point, PathWithCost>> paths)
        {
            var optimalPath = new List<Point>();
            var pathQueue = InitializePathQueue(gameState, paths);
            while (pathQueue.Count > 0)
            {
                var currentPath = pathQueue.Dequeue();
                var currentCost = currentPath.Cost;
                if (currentCost > gameState.InitialEnergy)
                    continue;

                optimalPath = currentPath.Path;
                foreach (var chest in gameState.Chests)
                {
                    if (!currentPath.Path.Contains(chest))
                    {
                        var extendedPath = new List<Point>(optimalPath);
                        extendedPath.AddRange(paths[currentPath.End][chest].Path.Skip(1));
                        var extendedPathCost = currentCost + paths[currentPath.End][chest].Cost;
                        pathQueue.Enqueue(new PathWithCost(extendedPathCost, extendedPath.ToArray()));
                    }
                }
            }
            return optimalPath.Skip(1).ToList();
        }
    }
}