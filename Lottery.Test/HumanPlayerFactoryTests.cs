using Lottery.Core.Factories;
using Lottery.Core.Models;

namespace Lottery.Test
{
    public class HumanPlayerFactoryTests
    {
        private readonly LotterySettings _settings;

        public HumanPlayerFactoryTests()
        {
            _settings = new LotterySettings
            {
                InitialBalance = 10m,
                MinPlayers = 3,
                MaxPlayers = 5
            };
        }

        [Fact]
        public void Create_ReturnsOnePlayer()
        {
            var sut = new HumanPlayerFactory(_settings, "Test");
            var players = sut.Create(1).ToList();
            Assert.Single(players);
        }

        [Fact]
        public void Create_ReturnsPlayerWithCorrectId()
        {
            var sut = new HumanPlayerFactory(_settings, "Test");
            var player = sut.Create(1).First();
            Assert.Equal(1, player.Id);
        }

        [Fact]
        public void Create_ReturnsPlayerWithCorrectName()
        {
            var sut = new HumanPlayerFactory(_settings, "TestPlayer");
            var player = sut.Create(1).First();
            Assert.Equal("TestPlayer", player.Name);
        }

        [Fact]
        public void Create_ReturnsPlayerWithCorrectBalance()
        {
            var sut = new HumanPlayerFactory(_settings, "Test");
            var player = sut.Create(1).First();
            Assert.Equal(_settings.InitialBalance, player.Balance);
        }

        [Fact]
        public void Create_ReturnsNonCpuPlayer()
        {
            var sut = new HumanPlayerFactory(_settings, "Test");
            var player = sut.Create(1).First();
            Assert.False(player.IsCPU);
        }

        [Fact]
        public void Create_UsesStartIdAsPlayerId()
        {
            var sut = new HumanPlayerFactory(_settings, "Test");
            var player = sut.Create(5).First();
            Assert.Equal(5, player.Id);
        }
    }
}
