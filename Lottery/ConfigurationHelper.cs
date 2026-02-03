using Lottery.Core.Models;
using Microsoft.Extensions.Configuration;

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
}
