using Lottery.Core.Models;

namespace Lottery.Core.Interfaces
{
    public interface ITicketService
    {
        void BuyTickets(Player player, int ticketCount);
    }
}
