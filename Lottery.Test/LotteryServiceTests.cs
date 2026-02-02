using Lottery.Core.Interfaces;
using Lottery.Core.Models;
using Lottery.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Lottery.Test
{
    public class LotteryServiceTests : IDisposable
    {
        private readonly ILotteryService _sut;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<IRandomGenerator> _mockRandom;

        public LotteryServiceTests()
        {
            _mockRandom = new Mock<IRandomGenerator>();
            _mockRandom.Setup(r => r.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(3);

            var settings = CreateSettings();
            _serviceProvider = BuildServiceProvider(settings, _mockRandom.Object);
            _sut = _serviceProvider.GetRequiredService<ILotteryService>();
        }

        public void Dispose()
        {
            _serviceProvider?.Dispose();
        }

        private static LotterySettings CreateSettings(decimal initialBalance = 10m, int minPlayers = 3, int maxPlayers = 5)
        {
            return new LotterySettings
            {
                InitialBalance = initialBalance,
                MinPlayers = minPlayers,
                MaxPlayers = maxPlayers
            };
        }

        private static ServiceProvider BuildServiceProvider(LotterySettings settings, IRandomGenerator random)
        {
            return new ServiceCollection()
                .AddSingleton(settings)
                .AddSingleton(random)
                .AddTransient<ILotteryService, LotteryService>()
                .BuildServiceProvider();
        }

        private static ILotteryService CreateService(LotterySettings settings, int cpuPlayerCount)
        {
            var mockRandom = new Mock<IRandomGenerator>();
            mockRandom.Setup(r => r.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(cpuPlayerCount);
            return BuildServiceProvider(settings, mockRandom.Object).GetRequiredService<ILotteryService>();
        }

        [Fact]
        public void GetPlayers_WhenInitialized_ReturnsEmptyCollection()
        {
            var players = _sut.GetPlayers();
            Assert.Empty(players);
        }

        [Fact]
        public void ExecuteDraw_ReturnsLotteryResult()
        {
            _sut.InitializePlayers("Test");
            var result = _sut.ExecuteDraw();
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData("John")]
        [InlineData("Alice")]
        [InlineData("Player 1")]
        public void InitializePlayers_CreatesHumanPlayerWithCorrectName(string playerName)
        {
            _sut.InitializePlayers(playerName);

            var humanPlayer = _sut.GetPlayers().First(p => !p.IsCPU);
            Assert.Equal(playerName, humanPlayer.Name);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(25)]
        [InlineData(100)]
        public void InitializePlayers_CreatesHumanPlayerWithCorrectBalance(decimal initialBalance)
        {
            var settings = CreateSettings(initialBalance: initialBalance);
            var sut = CreateService(settings, cpuPlayerCount: 3);

            sut.InitializePlayers("Test");

            var humanPlayer = sut.GetPlayers().First(p => !p.IsCPU);
            Assert.Equal(initialBalance, humanPlayer.Balance);
        }

        [Fact]
        public void InitializePlayers_HumanPlayerHasIdOne()
        {
            _sut.InitializePlayers("Test");

            var humanPlayer = _sut.GetPlayers().First(p => !p.IsCPU);
            Assert.Equal(1, humanPlayer.Id);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(10)]
        public void InitializePlayers_CreatesExactCpuPlayerCount(int expectedCount)
        {
            var settings = CreateSettings();
            var sut = CreateService(settings, cpuPlayerCount: expectedCount);

            sut.InitializePlayers("Test");

            var cpuCount = sut.GetPlayers().Count(p => p.IsCPU);
            Assert.Equal(expectedCount, cpuCount);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(50)]
        public void InitializePlayers_CpuPlayersHaveCorrectBalance(decimal initialBalance)
        {
            var settings = CreateSettings(initialBalance: initialBalance);
            var sut = CreateService(settings, cpuPlayerCount: 3);

            sut.InitializePlayers("Test");

            var cpuPlayers = sut.GetPlayers().Where(p => p.IsCPU);
            Assert.All(cpuPlayers, p => Assert.Equal(initialBalance, p.Balance));
        }

        [Fact]
        public void InitializePlayers_CpuPlayersHaveSequentialIds()
        {
            var settings = CreateSettings();
            var sut = CreateService(settings, cpuPlayerCount: 3);

            sut.InitializePlayers("Test");

            var cpuIds = sut.GetPlayers().Where(p => p.IsCPU).Select(p => p.Id).ToList();
            Assert.Equal(new[] { 2, 3, 4 }, cpuIds);
        }
    }
}
