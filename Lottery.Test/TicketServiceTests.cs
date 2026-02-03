using Lottery.Core.Models;
using Lottery.Core.Services;

namespace Lottery.Test
{
    public class TicketServiceTests
    {
        private readonly LotterySettings _settings;
        private readonly TicketService _sut;

        public TicketServiceTests()
        {
            _settings = new LotterySettings
            {
                TicketPrice = 1m,
                InitialBalance = 10m
            };

            _sut = new TicketService(_settings);
        }

        private static Player CreatePlayer(decimal balance = 10m)
        {
            return new Player
            {
                Id = 1,
                Name = "Test",
                Balance = balance
            };
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void BuyTickets_AddsCorrectNumberOfTickets(int ticketCount)
        {
            var player = CreatePlayer();

            _sut.BuyTickets(player, ticketCount);

            Assert.Equal(ticketCount, player.Tickets.Count);
        }

        [Theory]
        [InlineData(10, 1, 3, 7)]
        [InlineData(10, 2, 3, 4)]
        [InlineData(20, 1, 10, 10)]
        public void BuyTickets_DeductsCorrectAmountFromBalance(
            decimal initialBalance, decimal ticketPrice, int ticketCount, decimal expectedBalance)
        {
            var settings = new LotterySettings { TicketPrice = ticketPrice };
            var sut = new TicketService(settings);
            var player = CreatePlayer(initialBalance);

            sut.BuyTickets(player, ticketCount);

            Assert.Equal(expectedBalance, player.Balance);
        }

        [Fact]
        public void BuyTickets_StopsWhenBalanceInsufficient()
        {
            var player = CreatePlayer(balance: 3m);

            _sut.BuyTickets(player, 10);

            Assert.Equal(3, player.Tickets.Count);
            Assert.Equal(0m, player.Balance);
        }

        [Fact]
        public void BuyTickets_TicketsHaveCorrectPlayerId()
        {
            var player = CreatePlayer();

            _sut.BuyTickets(player, 3);

            Assert.All(player.Tickets, t => Assert.Equal(player.Id, t.PlayerId));
        }

        [Fact]
        public void BuyTickets_WithZeroCount_AddsNoTickets()
        {
            var player = CreatePlayer();

            _sut.BuyTickets(player, 0);

            Assert.Empty(player.Tickets);
            Assert.Equal(10m, player.Balance);
        }

        [Fact]
        public void BuyTickets_WithZeroBalance_AddsNoTickets()
        {
            var player = CreatePlayer(balance: 0m);

            _sut.BuyTickets(player, 5);

            Assert.Empty(player.Tickets);
        }

        [Fact]
        public void BuyTickets_EachTicketHasUniqueId()
        {
            var player = CreatePlayer();

            _sut.BuyTickets(player, 5);

            var ticketIds = player.Tickets.Select(t => t.Id).ToList();
            Assert.Equal(ticketIds.Count, ticketIds.Distinct().Count());
        }
    }
}
