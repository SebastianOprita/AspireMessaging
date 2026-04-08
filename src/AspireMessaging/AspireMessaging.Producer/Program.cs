using AspireMessaging.Contracts;
using AspireMessaging.Producer;
using MassTransit;
using System.ComponentModel.DataAnnotations;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// MassTransit + RabbitMQ configuration
builder.Services.AddMassTransit(x =>
{
    // If you have consumers, configure them here (x.AddConsumer<...>();)
    x.UsingRabbitMq((context, cfg) =>
    {
        var connectionString = builder.Configuration.GetConnectionString("rabbitmq")
            ?? throw new InvalidOperationException("Connection string 'rabbitmq' was not found.");

        cfg.Host(new Uri(connectionString));

        // Optional: configure publish topology or exchange names, message conventions, etc.
        cfg.Message<InvoiceSubmitted>(m => m.SetEntityName("invoices-submitted-exchange"));
        cfg.Message<PaymentSubmitted>(m => m.SetEntityName("payments-submitted-exchange"));
        cfg.Message<NotificationSubmitted>(m => m.SetEntityName("notifications-fanout-exchange"));

        cfg.Publish<NotificationSubmitted>(p =>
        {
            p.ExchangeType = RabbitMQ.Client.ExchangeType.Fanout;
            p.Durable = true;
        });

        cfg.Publish<InvoiceSubmitted>(p =>
        {
            p.ExchangeType = RabbitMQ.Client.ExchangeType.Direct;
            p.Durable = true; // optional extras
        });

        cfg.Publish<PaymentSubmitted>(p =>
        {
            p.ExchangeType = RabbitMQ.Client.ExchangeType.Direct;
            p.Durable = true; // optional extras
        });
    });
});

// If you want request validation helpers
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "demoApi v1");
    });
}

// Simple health endpoint
app.MapGet("/health", () => Results.Ok(new { status = "ok", time = DateTime.UtcNow }));

// POST /invoices
app.MapPost("/invoices", async (InvoiceDto invoiceDto, IPublishEndpoint publisher, ILogger<Program> logger) =>
{
    var validationResults = new List<ValidationResult>();
    var ctx = new ValidationContext(invoiceDto);
    if (!Validator.TryValidateObject(invoiceDto, ctx, validationResults, true))
    {
        return Results.BadRequest(validationResults);
    }

    var invoiceId = Guid.NewGuid();
    var invoiceDate = invoiceDto.InvoiceDate ?? DateTime.UtcNow;

    var message = new InvoiceSubmitted(
        InvoiceId: invoiceId,
        CustomerId: invoiceDto.CustomerId,
        Amount: invoiceDto.Amount,
        InvoiceDate: invoiceDate,
        Currency: invoiceDto.Currency,
        Description: invoiceDto.Description
    );

    try
    {
        await publisher.Publish<InvoiceSubmitted>(message);
        logger.LogInformation("Published InvoiceSubmitted {InvoiceId} for Customer {CustomerId}", invoiceId, invoiceDto.CustomerId);

        // Return 202 Accepted with location to a hypothetical resource
        return Results.Accepted($"/invoices/{invoiceId}", new { invoiceId });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to publish invoice {InvoiceId}", invoiceId);
        return Results.StatusCode(500);
    }
});

// POST /payments
app.MapPost("/payments", async (PaymentDto paymentDto, IPublishEndpoint publisher, ILogger<Program> logger) =>
{
    var validationResults = new List<ValidationResult>();
    var ctx = new ValidationContext(paymentDto);
    if (!Validator.TryValidateObject(paymentDto, ctx, validationResults, true))
    {
        return Results.BadRequest(validationResults);
    }

    var paymentId = Guid.NewGuid();
    var paymentDate = paymentDto.PaymentDate ?? DateTime.UtcNow;

    var message = new PaymentSubmitted(
        PaymentId: paymentId,
        InvoiceId: paymentDto.InvoiceId,
        Amount: paymentDto.Amount,
        PaymentDate: paymentDate,
        Currency: paymentDto.Currency,
        Method: paymentDto.Method
    );

    try
    {
        await publisher.Publish<PaymentSubmitted>(message);
        logger.LogInformation("Published PaymentSubmitted {PaymentId} for Invoice {InvoiceId}", paymentId, paymentDto.InvoiceId);

        return Results.Accepted($"/payments/{paymentId}", new { paymentId });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to publish payment {PaymentId}", paymentId);
        return Results.StatusCode(500);
    }
});

// POST /notification
app.MapPost("/notification", async (AspireMessaging.Producer.Dtos.NotificationDto notificationDto, IPublishEndpoint publisher, ILogger<Program> logger) =>
{
    var validationResults = new List<ValidationResult>();
    var ctx = new ValidationContext(notificationDto);
    if (!Validator.TryValidateObject(notificationDto, ctx, validationResults, true))
    {
        return Results.BadRequest(validationResults);
    }

    var message = new NotificationSubmitted(
        NotificationId: notificationDto.NotificationId,
        InvoiceId: notificationDto.InvoiceId,
        PaymentId: notificationDto.PaymentId,
        PaymentDate: notificationDto.PaymentDate,
        Description: notificationDto.Description
    );

    try
    {
        await publisher.Publish<NotificationSubmitted>(message);
        logger.LogInformation("Published NotificationSubmitted {NotificationId}", notificationDto.NotificationId);
        return Results.Accepted($"/notification/{notificationDto.NotificationId}", new { notificationDto.NotificationId });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to publish notification {NotificationId}", notificationDto.NotificationId);
        return Results.StatusCode(500);
    }
});

app.Run();