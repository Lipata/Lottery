var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure (containers) with persistent volumes
var redis = builder.AddRedis("redis")
    .WithDataVolume("redis-data");

var rabbitMq = builder.AddRabbitMQ("rabbitmq")
    .WithDataVolume("rabbitmq-data")
    .WithManagementPlugin();

var kafka = builder.AddKafka("kafka")
    .WithDataVolume("kafka-data");

// API — references all infrastructure
var api = builder.AddProject<Projects.Lottery_Api>("lottery-api")
    .WithReference(redis).WaitFor(redis)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithReference(kafka).WaitFor(kafka)
    .WithReplicas(2);

// Web — references API via service discovery
builder.AddProject<Projects.Lottery_Web>("lottery-web")
    .WithExternalHttpEndpoints()
    .WithReference(api).WaitFor(api);

// Payment — consumes RabbitMQ messages
builder.AddProject<Projects.Lottery_Payment>("lottery-payment")
    .WithReference(rabbitMq).WaitFor(rabbitMq);

builder.Build().Run();
