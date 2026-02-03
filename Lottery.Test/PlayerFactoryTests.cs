using Lottery.Core.Interfaces;
using Lottery.Core.Models;
using Lottery.Core.Services;
using Moq;

namespace Lottery.Test
{
    public class PlayerFactoryTests
    {
        private readonly Mock<IRandomGenerator> _mockRandom;
        private readonly LotterySettings _settings;
        private readonly PlayerFactory _sut;

        public PlayerFactoryTests()
        {
            _mockRandom = new Mock<IRandomGenerator>();
            _mockRandom.Setup(r => r.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(3);

            _settings = new LotterySettings
            {
                InitialBalance = 10m,
                MinPlayers = 3,
                MaxPlayers = 5
            };

            _sut = new PlayerFactory(_settings, _mockRandom.Object);
        }

        [Fact]
        public void CreateHumanPlayer_ReturnsPlayerWithCorrectId()
        {
            var player = _sut.CreateHumanPlayer("Test");
            Assert.Equal(1, player.Id);
        }

        [Fact]
        public void CreateHumanPlayer_ReturnsPlayerWithCorrectName()
        {
            var player = _sut.CreateHumanPlayer("TestPlayer");
            Assert.Equal("TestPlayer", player.Name);
        }

        [Fact]
        public void CreateHumanPlayer_ReturnsPlayerWithCorrectBalance()
        {
            var player = _sut.CreateHumanPlayer("Test");
            Assert.Equal(_settings.InitialBalance, player.Balance);
        }

        [Fact]
        public void CreateHumanPlayer_ReturnsNonCpuPlayer()
        {
            var player = _sut.CreateHumanPlayer("Test");
            Assert.False(player.IsCPU);
        }

        [Fact]
        public void CreateCpuPlayers_ReturnsCorrectCount()
        {
            _mockRandom.Setup(r => r.Next(_settings.MinPlayers, _settings.MaxPlayers + 1)).Returns(5);

            var players = _sut.CreateCpuPlayers(2).ToList();

            Assert.Equal(5, players.Count);
        }

        [Fact]
        public void CreateCpuPlayers_ReturnsPlayersWithSequentialIds()
        {
            _mockRandom.Setup(r => r.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(3);

            var players = _sut.CreateCpuPlayers(2).ToList();

            Assert.Equal(new[] { 2, 3, 4 }, players.Select(p => p.Id));
        }

        [Fact]
        public void CreateCpuPlayers_ReturnsPlayersWithCorrectBalance()
        {
            var players = _sut.CreateCpuPlayers(2).ToList();

            Assert.All(players, p => Assert.Equal(_settings.InitialBalance, p.Balance));
        }

        [Fact]
        public void CreateCpuPlayers_ReturnsCpuPlayers()
        {
            var players = _sut.CreateCpuPlayers(2).ToList();

            Assert.All(players, p => Assert.True(p.IsCPU));
        }

        [Theory]
        [InlineData(2, "Player 2")]
        [InlineData(5, "Player 5")]
        [InlineData(10, "Player 10")]
        public void CreateCpuPlayers_ReturnsPlayersWithCorrectNameFormat(int startId, string expectedFirstName)
        {
            _mockRandom.Setup(r => r.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(1);

            var players = _sut.CreateCpuPlayers(startId).ToList();

            Assert.Equal(expectedFirstName, players.First().Name);
        }
    }
}
