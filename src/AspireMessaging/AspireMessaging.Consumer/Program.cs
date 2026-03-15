using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AspireMessaging.Contracts;
using AspireMessaging.Consumer;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<InvoiceSubmittedConsumer>();
    x.AddConsumer<PaymentSubmittedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var connectionString = builder.Configuration.GetConnectionString("rabbitmq")
            ?? throw new InvalidOperationException("Connection string 'rabbitmq' was not found.");

        cfg.Host(new Uri(connectionString));

        cfg.Message<InvoiceSubmitted>(m => m.SetEntityName("invoices-submitted-exchange"));
        cfg.Message<PaymentSubmitted>(m => m.SetEntityName("payments-submitted-exchange"));

        // receive endpoint for invoices
        cfg.ReceiveEndpoint("invoice-submitted-queue", e =>
        {
            e.ConfigureConsumer<InvoiceSubmittedConsumer>(context);
        });

        // receive endpoint for payments
        cfg.ReceiveEndpoint("payment-submitted-queue", e =>
        {
            e.ConfigureConsumer<PaymentSubmittedConsumer>(context);
        });
    });
});

var host = builder.Build();

Console.WriteLine("Consumer app started. Waiting for messages...");
await host.RunAsync();