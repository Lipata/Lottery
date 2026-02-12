using Lottery.Contracts;

namespace Lottery.Core.Models
{
    public record Player : PlayerDto
    {
        public List<Ticket> Tickets { get; init; } = new();
    }
}
