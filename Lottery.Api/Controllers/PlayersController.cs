using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Lottery.Contracts;
using Lottery.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using RabbitMQ.Client;

namespace Lottery.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController(
    LotteryDbContext db,
    IDistributedCache cache,
    IConnection rabbitConnection,
    IProducer<string, string> kafkaProducer) : ControllerBase
{
    private const string PlayersCacheKey = "players:all";
    private const string KafkaTopic = "lottery-domain-events";
    private const string RabbitQueue = "tickets-purchased";
    private const decimal TicketPrice = 10m;

    [HttpGet]
    public async Task<ActionResult<List<PlayerDto>>> GetAll()
    {
        var cached = await cache.GetAsync(PlayersCacheKey);
        if (cached is not null)
        {
            var cachedPlayers = JsonSerializer.Deserialize<List<PlayerDto>>(cached);
            return Ok(cachedPlayers);
        }

        var players = await db.Players.ToListAsync();

        await cache.SetAsync(PlayersCacheKey, JsonSerializer.SerializeToUtf8Bytes(players),
            new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) });

        return Ok(players);
    }

    [HttpPost]
    public async Task<ActionResult<PlayerDto>> Create([FromBody] CreatePlayerRequest request)
    {
        var player = new PlayerDto
        {
            Name = request.Name,
            IsCPU = false,
            Balance = 0
        };

        db.Players.Add(player);
        await db.SaveChangesAsync();

        await cache.RemoveAsync(PlayersCacheKey);

        var domainEvent = new PlayerRegisteredEvent
        {
            PlayerId = player.Id,
            PlayerName = player.Name,
            Timestamp = DateTime.UtcNow
        };
        await kafkaProducer.ProduceAsync(KafkaTopic,
            new Message<string, string>
            {
                Key = player.Id.ToString(),
                Value = JsonSerializer.Serialize(domainEvent)
            });

        return CreatedAtAction(nameof(GetAll), new { id = player.Id }, player);
    }

    [HttpPost("{id}/tickets")]
    public async Task<IActionResult> PurchaseTickets(int id, [FromBody] PurchaseTicketsRequest request)
    {
        var player = await db.Players.FindAsync(id);
        if (player is null)
            return NotFound();

        var totalCost = request.TicketCount * TicketPrice;
        if (player.Balance < totalCost)
            return BadRequest("Insufficient balance.");

        player.Balance -= totalCost;
        await db.SaveChangesAsync();

        await cache.RemoveAsync(PlayersCacheKey);

        // Publish command to RabbitMQ for payment processing
        var message = new TicketsPurchasedMessage
        {
            PlayerId = id,
            TicketCount = request.TicketCount,
            TotalCost = totalCost,
            Timestamp = DateTime.UtcNow
        };

        await using var channel = await rabbitConnection.CreateChannelAsync();
        await channel.QueueDeclareAsync(RabbitQueue, durable: true, exclusive: false, autoDelete: false);
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        await channel.BasicPublishAsync("", RabbitQueue, body);

        // Publish domain event to Kafka for audit/analytics
        var domainEvent = new TicketsPurchasedEvent
        {
            PlayerId = id,
            TicketCount = request.TicketCount,
            TotalCost = totalCost,
            Timestamp = DateTime.UtcNow
        };
        await kafkaProducer.ProduceAsync(KafkaTopic,
            new Message<string, string>
            {
                Key = id.ToString(),
                Value = JsonSerializer.Serialize(domainEvent)
            });

        return Ok(new { message = $"Purchased {request.TicketCount} ticket(s) for {totalCost:C}" });
    }
}
