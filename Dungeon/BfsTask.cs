using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon;

public class BfsTask
{
    public static IEnumerable<SinglyLinkedList<Point>> FindPaths(Map dungeonMap, Point startPoint, Chest[] chests)
    {
        var chestLocations = chests.Select(chest => chest.Location).ToHashSet();
        var pathQueue = new Queue<SinglyLinkedList<Point>>();
        var startNode = new SinglyLinkedList<Point>(startPoint);
        pathQueue.Enqueue(startNode);
        var visitedPoints = new HashSet<Point> { startPoint };

        while (pathQueue.Count != 0)
        {
            var currentNode = pathQueue.Dequeue();
            foreach (var nextPoint in GetNextNodes(currentNode, dungeonMap))
            {
                if (dungeonMap.Dungeon[nextPoint.X, nextPoint.Y] == MapCell.Wall || visitedPoints.Contains(nextPoint))
                    continue;

                var nextNode = new SinglyLinkedList<Point>(nextPoint, currentNode);
                visitedPoints.Add(nextPoint);
                pathQueue.Enqueue(nextNode);

                if (chestLocations.Contains(nextPoint))
                    yield return nextNode;
            }
        }
    }

    private static IEnumerable<Point> GetNextNodes(SinglyLinkedList<Point> currentNode, Map dungeonMap)
    {
        var point = currentNode.Value;
        return Walker.PossibleDirections
            .Select(direction => direction + point)
            .Where(dungeonMap.InBounds);
    }
}