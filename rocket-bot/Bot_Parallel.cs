using System;
using System.Collections.Generic;
using System.Linq;

namespace rocket_bot
{
    public partial class Bot
    {
        public Rocket GetNextMove(Rocket rocket)
        {
            return rocket.Move(FindBestMove(rocket), level);
        }

        private Turn FindBestMove(Rocket rocket)
        {
            var moves = 
                DistributeNumber(iterationsCount, threadsCount).AsParallel()
                .Select(cnt => SearchBestMove(rocket, new Random(NextRandom()), cnt))
                .ToList();
            var bestScore = moves.Max(m => m.Item2);
            return moves.First(m => m.Item2 == bestScore).Item1;
        }

        private static IEnumerable<int> DistributeNumber(int number, int chunksNumber)
        {
            if (chunksNumber == 0)
            {
                yield return 0;
                yield break;
            }

            var rest = number % chunksNumber;
            var chunk = number / chunksNumber;
            for (int i = 0; i < chunksNumber; i++)
            {
                yield return rest > 0 ? chunk + 1 : chunk;
                rest--;
            }
        }

        private int NextRandom()
        {
            lock (random)
            {
                return random.Next();
            }
        }
    }
}