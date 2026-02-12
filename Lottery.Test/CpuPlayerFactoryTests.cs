using Lottery.Core.Factories;
using Lottery.Core.Interfaces;
using Lottery.Core.Models;
using Moq;

namespace Lottery.Test
{
    public class CpuPlayerFactoryTests
    {
        private readonly Mock<IRandomGenerator> _mockRandom;
        private readonly LotterySettings _settings;
        private readonly CpuPlayerFactory _sut;

        public CpuPlayerFactoryTests()
        {
            _mockRandom = new Mock<IRandomGenerator>();
            _mockRandom.Setup(r => r.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(3);

            _settings = new LotterySettings
            {
                InitialBalance = 10m,
                MinPlayers = 3,
                MaxPlayers = 5
            };

            _sut = new CpuPlayerFactory(_settings, _mockRandom.Object);
        }

        [Fact]
        public void Create_ReturnsCorrectCount()
        {
            _mockRandom.Setup(r => r.Next(_settings.MinPlayers, _settings.MaxPlayers + 1)).Returns(5);

            var players = _sut.Create(2).ToList();

            Assert.Equal(5, players.Count);
        }

        [Fact]
        public void Create_ReturnsPlayersWithSequentialIds()
        {
            _mockRandom.Setup(r => r.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(3);

            var players = _sut.Create(2).ToList();

            Assert.Equal(new[] { 2, 3, 4 }, players.Select(p => p.Id));
        }

        [Fact]
        public void Create_ReturnsPlayersWithCorrectBalance()
        {
            var players = _sut.Create(2).ToList();

            Assert.All(players, p => Assert.Equal(_settings.InitialBalance, p.Balance));
        }

        [Fact]
        public void Create_ReturnsCpuPlayers()
        {
            var players = _sut.Create(2).ToList();

            Assert.All(players, p => Assert.True(p.IsCPU));
        }

        [Theory]
        [InlineData(2, "Player 2")]
        [InlineData(5, "Player 5")]
        [InlineData(10, "Player 10")]
        public void Create_ReturnsPlayersWithCorrectNameFormat(int startId, string expectedFirstName)
        {
            _mockRandom.Setup(r => r.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(1);

            var players = _sut.Create(startId).ToList();

            Assert.Equal(expectedFirstName, players.First().Name);
        }
    }
}
