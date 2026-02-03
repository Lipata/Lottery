using Lottery.Core.Interfaces;
using Lottery.Core.Models;
using Lottery.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lottery;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLotteryServices(
        this IServiceCollection services,
        LotterySettings settings)
    {
        return services
            .AddSingleton(settings)
            .AddSingleton<IRandomGenerator, RandomGenerator>()
            .AddTransient<IPlayerFactory, PlayerFactory>()
            .AddTransient<ITicketService, TicketService>()
            .AddTransient<ILotteryService, LotteryService>();
    }
}
