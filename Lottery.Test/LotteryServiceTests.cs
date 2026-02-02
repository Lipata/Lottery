using Lottery.Core.Interfaces;
using Lottery.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lottery.Test
{
    public class LotteryServiceTests : IDisposable
    {
        private readonly ILotteryService _sut;
        private readonly ServiceProvider _serviceProvider;

        public LotteryServiceTests()
        {
            var services = new ServiceCollection();
            services.AddScoped<ILotteryService, LotteryService>();

            _serviceProvider = services.BuildServiceProvider();
            _sut = _serviceProvider.GetRequiredService<ILotteryService>();
        }

        public void Dispose()
        {
            _serviceProvider?.Dispose();
        }

        [Fact]
        public void GetPlayers_WhenInitialized_ReturnsEmptyCollection()
        {
            var players = _sut.GetPlayers();
            Assert.Empty(players);
        }

        [Fact]
        public void InitializeGame_ClearsPlayers()
        {
            _sut.InitializeGame();
            var players = _sut.GetPlayers();
            Assert.Empty(players);
        }

        [Fact]
        public void ExecuteDraw_WhenNoPlayers_ReturnsZeroRevenue()
        {
            _sut.InitializeGame();
            var result = _sut.ExecuteDraw();
            Assert.Equal(0m, result.Revenue);
        }

        [Fact]
        public void ExecuteDraw_WhenNoPlayers_ReturnsZeroProfit()
        {
            _sut.InitializeGame();
            var result = _sut.ExecuteDraw();
            Assert.Equal(0m, result.Profit);
        }

        [Fact]
        public void ExecuteDraw_WhenNoPlayers_ReturnsEmptyWinners()
        {
            _sut.InitializeGame();
            var result = _sut.ExecuteDraw();
            Assert.Empty(result.Winners);
        }

        [Fact]
        public void ExecuteDraw_ReturnsLotteryResult()
        {
            _sut.InitializeGame();
            var result = _sut.ExecuteDraw();
            Assert.NotNull(result);
        }
    }
}
