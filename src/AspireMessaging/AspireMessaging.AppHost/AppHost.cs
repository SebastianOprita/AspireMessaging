var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin(port: 15672);

builder.AddProject<Projects.AspireMessaging_Producer>("Producer")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.AddProject<Projects.AspireMessaging_Consumer>("Consumer")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.Build().Run();