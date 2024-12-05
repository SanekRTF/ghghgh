using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rocket_bot
{
    public partial class Bot
    {
        public Rocket GetNextMove(Rocket currentRocket)
        {
            var (bestTurn, score) = CreateTasks(currentRocket)
                .First()
                .GetAwaiter()
                .GetResult();

            return currentRocket.Move(bestTurn, level);
        }

        public List<Task<(Turn BestTurn, double Score)>> CreateTasks(Rocket currentRocket)
        {
            var tasks = new List<Task<(Turn BestTurn, double Score)>>();
            for (var i = 0; i < threadsCount; i++)
            {
                var task = Task.Run(() =>
                    SearchBestMove(currentRocket, new Random(random.Next()), iterationsCount / threadsCount));
                tasks.Add(task);
            }
            return tasks;
        }
    }
}