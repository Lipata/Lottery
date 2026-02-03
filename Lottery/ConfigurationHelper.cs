using Lottery.Core.Interfaces;
using Lottery.Core.Models;
using Lottery.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lottery;

public static class ConfigurationHelper
{
    public static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    public static LotterySettings GetLotterySettings(IConfiguration configuration)
    {
        return configuration.GetSection("LotterySettings").Get<LotterySettings>()
            ?? throw new InvalidOperationException("LotterySettings configuration is missing");
    }

    public static ServiceProvider BuildServiceProvider(IConfiguration configuration)
    {
        return new ServiceCollection()
            .AddSingleton(GetLotterySettings(configuration))
            .AddSingleton<IRandomGenerator, RandomGenerator>()
            .AddTransient<IPlayerFactory, PlayerFactory>()
            .AddTransient<ITicketService, TicketService>()
            .AddTransient<ILotteryService, LotteryService>()
            .BuildServiceProvider();
    }
}
