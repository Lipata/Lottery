using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Lottery.Core.Interfaces;
using Lottery.Core.Models;
using Lottery.Core.Services;

var configuration = BuildConfiguration();

using var serviceProvider = BuildServiceProvider(configuration);
var lotteryService = serviceProvider.GetRequiredService<ILotteryService>();

Console.Write("Enter your name: ");
var playerName = Console.ReadLine() ?? "Player";
lotteryService.InitializePlayers(playerName);

Console.Write("How many tickets do you want to buy? ");
var ticketCount = int.Parse(Console.ReadLine() ?? "1");
lotteryService.BuyTickets(ticketCount);

var result = lotteryService.ExecuteDraw();

Console.WriteLine($"Players: {lotteryService.GetPlayers().Count()}");
Console.WriteLine($"Revenue: {result.Revenue:C}");
Console.WriteLine($"Profit: {result.Profit:C}");

static IConfiguration BuildConfiguration()
{
    return new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();
}

static ServiceProvider BuildServiceProvider(IConfiguration configuration)
{
    var lotterySettings = configuration.GetSection("LotterySettings").Get<LotterySettings>()
        ?? throw new InvalidOperationException("LotterySettings configuration is missing");

    return new ServiceCollection()
        .AddSingleton(lotterySettings)
        .AddSingleton<IRandomGenerator, RandomGenerator>()
        .AddTransient<ILotteryService, LotteryService>()
        .BuildServiceProvider();
}
