using Lottery.Core.Interfaces;
using Lottery.Core.Models;

namespace Lottery.Core.Services
{
    public class TicketService : ITicketService
    {
        private readonly LotterySettings _settings;

        public TicketService(LotterySettings settings)
        {
            _settings = settings;
        }

        public void BuyTickets(Player player, int ticketCount)
        {
            for (int i = 0; i < ticketCount; i++)
            {
                if (player.Balance < _settings.TicketPrice)
                    break;

                player.Balance -= _settings.TicketPrice;
                player.Tickets.Add(new Ticket { PlayerId = player.Id });
            }
        }
    }
}
