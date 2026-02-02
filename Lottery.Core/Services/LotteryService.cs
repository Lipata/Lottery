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

        public void InitializePlayers(string humanPlayerName)
        {
            _players.Clear();

            _players.Add(CreateHumanPlayer(humanPlayerName));
            _players.AddRange(GenerateCpuPlayers());
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

        private Player CreateHumanPlayer(string name)
        {
            return new Player
            {
                Id = 1,
                Name = name,
                Balance = _settings.InitialBalance,
                IsCPU = false
            };
        }

        private IEnumerable<Player> GenerateCpuPlayers()
        {
            var playerCount = Random.Shared.Next(_settings.MinPlayers, _settings.MaxPlayers + 1);
            var startId = _players.Count + 1;

            return Enumerable.Range(startId, playerCount)
                .Select(i => new Player
                {
                    Id = i,
                    Name = $"Player {i}",
                    Balance = _settings.InitialBalance
                });
        }
    }
}
