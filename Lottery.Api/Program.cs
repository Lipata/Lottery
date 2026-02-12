using Lottery.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddRedisDistributedCache("redis");
builder.AddRabbitMQClient("rabbitmq");
builder.AddKafkaProducer<string, string>("kafka");

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<LotteryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LotteryDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
