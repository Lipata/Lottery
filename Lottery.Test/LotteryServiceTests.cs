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

        private static LotterySettings CreateSettings(
            decimal initialBalance = 10m,
            decimal ticketPrice = 1m,
            int minPlayers = 3,
            int maxPlayers = 5,
            int minTicketsPerPlayer = 1,
            int maxTicketsPerPlayer = 10)
        {
            return new LotterySettings
            {
                InitialBalance = initialBalance,
                TicketPrice = ticketPrice,
                MinPlayers = minPlayers,
                MaxPlayers = maxPlayers,
                MinTicketsPerPlayer = minTicketsPerPlayer,
                MaxTicketsPerPlayer = maxTicketsPerPlayer
            };
        }

        private static ServiceProvider BuildServiceProvider(LotterySettings settings, IRandomGenerator random)
        {
            return new ServiceCollection()
                .AddSingleton(settings)
                .AddSingleton(random)
                .AddTransient<IPlayerFactory, PlayerFactory>()
                .AddTransient<ITicketService, TicketService>()
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

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void BuyTickets_HumanPlayerBuysExactTicketCount(int ticketCount)
        {
            _sut.InitializePlayers("Test");

            _sut.BuyTickets(ticketCount);

            var humanPlayer = _sut.GetPlayers().First(p => !p.IsCPU);
            Assert.Equal(ticketCount, humanPlayer.Tickets.Count);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        public void BuyTickets_CpuPlayersBuyRandomTicketCount(int expectedTicketCount)
        {
            var settings = CreateSettings();
            var sut = CreateService(settings, cpuPlayerCount: expectedTicketCount);

            sut.InitializePlayers("Test");
            sut.BuyTickets(1);

            var cpuPlayers = sut.GetPlayers().Where(p => p.IsCPU);
            Assert.All(cpuPlayers, p => Assert.Equal(expectedTicketCount, p.Tickets.Count));
        }

        [Theory]
        [InlineData(10, 1, 3, 7)]
        [InlineData(20, 2, 5, 10)]
        public void BuyTickets_DeductsCorrectAmountFromBalance(
            decimal initialBalance, decimal ticketPrice, int ticketCount, decimal expectedBalance)
        {
            var settings = CreateSettings(initialBalance: initialBalance, ticketPrice: ticketPrice);
            var sut = CreateService(settings, cpuPlayerCount: 1);

            sut.InitializePlayers("Test");
            sut.BuyTickets(ticketCount);

            var humanPlayer = sut.GetPlayers().First(p => !p.IsCPU);
            Assert.Equal(expectedBalance, humanPlayer.Balance);
        }

        [Fact]
        public void BuyTickets_StopsWhenBalanceInsufficient()
        {
            var settings = CreateSettings(initialBalance: 3m, ticketPrice: 1m);
            var sut = CreateService(settings, cpuPlayerCount: 1);

            sut.InitializePlayers("Test");
            sut.BuyTickets(10);

            var humanPlayer = sut.GetPlayers().First(p => !p.IsCPU);
            Assert.Equal(3, humanPlayer.Tickets.Count);
            Assert.Equal(0m, humanPlayer.Balance);
        }

        [Fact]
        public void BuyTickets_TicketsHaveCorrectPlayerId()
        {
            _sut.InitializePlayers("Test");

            _sut.BuyTickets(3);

            var humanPlayer = _sut.GetPlayers().First(p => !p.IsCPU);
            Assert.All(humanPlayer.Tickets, t => Assert.Equal(humanPlayer.Id, t.PlayerId));
        }
    }
}
