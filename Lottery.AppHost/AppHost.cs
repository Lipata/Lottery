var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure (containers)
var redis = builder.AddRedis("redis");
var rabbitMq = builder.AddRabbitMQ("rabbitmq");
var kafka = builder.AddKafka("kafka");

// API — references all infrastructure
var api = builder.AddProject<Projects.Lottery_Api>("lottery-api")
    .WithReference(redis)
    .WithReference(rabbitMq)
    .WithReference(kafka);

// Web — references API via service discovery
builder.AddProject<Projects.Lottery_Web>("lottery-web")
    .WithExternalHttpEndpoints()
    .WithReference(api);

// Payment — references RabbitMQ (stub listener)
builder.AddProject<Projects.Lottery_Payment>("lottery-payment")
    .WithReference(rabbitMq);

builder.Build().Run();
