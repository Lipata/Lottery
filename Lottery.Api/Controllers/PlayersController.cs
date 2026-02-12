using Lottery.Contracts;
using Lottery.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController(LotteryDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<PlayerDto>>> GetAll()
    {
        var players = await db.Players.ToListAsync();
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

        return CreatedAtAction(nameof(GetAll), new { id = player.Id }, player);
    }
}
