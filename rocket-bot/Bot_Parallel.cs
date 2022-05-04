using System;
using System.Linq;
using System.Threading.Tasks;

namespace rocket_bot
{
    public partial class Bot
    {
        public Rocket GetNextMove(Rocket rocket)
        {
            // TODO: распараллелить запуск SearchBestMove
            var bestMove = SearchBestMove(rocket, new Random(random.Next()), iterationsCount);
            var newRocket = rocket.Move(bestMove.Item1, level);
            return newRocket;
        }
    }
}