using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon
{
    public class DungeonTask
    {
        public static MoveDirection[] FindShortestPath(Map dungeonMap)
        {
            var exitChest = new Chest(dungeonMap.Exit, 0);
            var playerToChestPaths = BfsTask.FindPaths(dungeonMap, dungeonMap.InitialPosition, dungeonMap.Chests);
            var chestToExitPaths = BfsTask.FindPaths(dungeonMap, dungeonMap.Exit, dungeonMap.Chests);
            var playerToExitPath = BfsTask.FindPaths(dungeonMap, dungeonMap.InitialPosition, new[] { exitChest }).FirstOrDefault();

            if (playerToExitPath == null)
                return Array.Empty<MoveDirection>();

            if (!chestToExitPaths.Any() || !playerToChestPaths.Any())
                return Convert(playerToExitPath.Reverse());

            var combinedPaths = JoinPaths(playerToChestPaths, chestToExitPaths, dungeonMap);
            return combinedPaths
                .OrderByDescending(x => x.Item1)
                .ThenBy(x => x.Item2)
                .Select(x => Convert(x.Item3))
                .Last();
        }

        private static IEnumerable<(int, byte chestValue, IEnumerable<Point> path)> JoinPaths(
            IEnumerable<SinglyLinkedList<Point>> playerToChestPaths,
            IEnumerable<SinglyLinkedList<Point>> chestToExitPaths,
            Map dungeonMap)
        {
            var locationToChest = dungeonMap.Chests
                .ToDictionary(chest => chest.Location, chest => chest);

            return playerToChestPaths.Join(chestToExitPaths,
                path1 => path1.Value,
                path2 => path2.Value,
                (path1, path2) =>
                {
                    var playerToChestPath = path1.Reverse().SkipLast(1);
                    var chestToExitPath = path2;
                    var chestValue = locationToChest[path2.Value].Value;
                    var combinedPath = playerToChestPath.Concat(chestToExitPath);
                    return (path1.Length + path2.Length, chestValue, combinedPath);
                });
        }

        private static MoveDirection[] Convert(IEnumerable<Point> path)
        {
            var tempPath = path.Skip(1);
            return path.Zip(tempPath, (start, end) => end - start)
                .Select(offset => Walker.ConvertOffsetToDirection(offset))
                .ToArray();
        }
    }
}