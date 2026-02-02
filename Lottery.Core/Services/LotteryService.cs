using Lottery.Core.Interfaces;
using Lottery.Core.Models;

namespace Lottery.Core.Services
{
    public class LotteryService : ILotteryService
    {
        private readonly List<Player> _players = new();
        private readonly LotterySettings _settings;

        public LotteryService(LotterySettings settings)
        {
            _settings = settings;
        }

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

            GeneratePlayers();

            return new LotteryResult
            {
                Revenue = 0m,
                Profit = 0m,
                Winners = new List<WinnerDisplayInfo>()
            };
        }

        private List<Player> GeneratePlayers()
        {
            var playerCount = Random.Shared.Next(_settings.MinPlayers, _settings.MaxPlayers + 1);

            var players =
                Enumerable.Range(1, playerCount)
                .Select(i => new Player
                {
                    Id = i,
                    Name = $"Player {i}",
                    Balance = _settings.InitialBalance
                })
                .ToList();

            return players;
        }
    }
}
