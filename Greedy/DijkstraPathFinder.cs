using System;
using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy
{
    public class DijkstraPathFinder
    {
        public IEnumerable<PathWithCost> GetPathsByDijkstra(State gameState, Point startPoint, IEnumerable<Point> targetPoints)
        {
            var visitedTargets = new HashSet<Point>();
            var visitedPoints = new HashSet<Point>();
            var track = new Dictionary<Point, DijkstraData>();
            track[startPoint] = new DijkstraData(0, null);

            while (visitedTargets.Count < targetPoints.Count())
            {
                if (!TryGetMinDifficultPoint(track, out var currentPoint, visitedPoints))
                    yield break;

                foreach (var neighbourPoint in GetNeighbours(gameState, currentPoint))
                {
                    var currentDifficult = gameState.CellCost[neighbourPoint.X, neighbourPoint.Y] + track[currentPoint].Difficult;
                    if (!track.ContainsKey(neighbourPoint) || track[neighbourPoint].Difficult > currentDifficult)
                        track[neighbourPoint] = new DijkstraData(currentDifficult, currentPoint);
                }

                visitedPoints.Add(currentPoint);
                if (targetPoints.Contains(currentPoint))
                {
                    visitedTargets.Add(currentPoint);
                    yield return GetPathWithCost(currentPoint, track);
                }
            }
        }

        private bool TryGetMinDifficultPoint(Dictionary<Point, DijkstraData> track, out Point minDifficultPoint, HashSet<Point> visitedPoints)
        {
            var bestDifficult = int.MaxValue;
            var found = false;
            minDifficultPoint = new Point(0, 0);

            foreach (var point in track.Keys)
            {
                if (!visitedPoints.Contains(point) && track[point].Difficult < bestDifficult)
                {
                    found = true;
                    minDifficultPoint = point;
                    bestDifficult = track[point].Difficult;
                }
            }
            return found;
        }

        private PathWithCost GetPathWithCost(Point endPoint, Dictionary<Point, DijkstraData> track)
        {
            var totalDifficult = track[endPoint].Difficult;
            Point? currentPoint = endPoint;
            var path = new List<Point>();

            while (currentPoint != null)
            {
                path.Add(currentPoint.Value);
                currentPoint = track[currentPoint.Value].Previous;
            }

            path.Reverse();
            return new PathWithCost(totalDifficult, path.ToArray());
        }

        private IEnumerable<Point> GetNeighbours(State gameState, Point startPoint)
        {
            var directionOffsets = new[] { -1, 0, 1 };
            return directionOffsets.SelectMany(dx =>
                directionOffsets.Select(dy => new Point(dx, dy)))
                .Where(p => Math.Abs(p.X) + Math.Abs(p.Y) == 1)
                .Select(p => p + startPoint)
                .Where(p => gameState.InsideMap(p) && !p.Equals(startPoint) && !gameState.IsWallAt(p));
        }

        private class DijkstraData
        {
            public Point? Previous { get; }
            public int Difficult { get; }

            public DijkstraData(int difficult, Point? previous)
            {
                Difficult = difficult;
                Previous = previous;
            }
        }
    }
}