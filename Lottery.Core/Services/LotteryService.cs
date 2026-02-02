using Lottery.Core.Interfaces;
using Lottery.Core.Models;

namespace Lottery.Core.Services
{
    internal class LotteryService : ILotteryService
    {
        private readonly List<Player> _players = new();

        public void InitializeGame()
        {
            _players.Clear();
        }

        public IEnumerable<Player> GetPlayers()
        {
            return _players;
        }

        public LotteryResult ExecuteDraw()
        {

            return new LotteryResult
            {
                Revenue = 0m,
                Profit = 0m,
                Winners = new List<WinnerDisplayInfo>()
            };
        }
    }
}
