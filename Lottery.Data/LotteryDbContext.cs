using Lottery.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Data;

public class LotteryDbContext(DbContextOptions<LotteryDbContext> options) : DbContext(options)
{
    public DbSet<PlayerDto> Players => Set<PlayerDto>();
}
