using AspireMessaging.Contracts;
using MassTransit;

namespace AspireMessaging.Consumer;

public class InvoiceSubmittedConsumer : IConsumer<InvoiceSubmitted>
{
    public Task Consume(ConsumeContext<InvoiceSubmitted> context)
    {
        var msg = context.Message;

        Console.WriteLine("=== INVOICE RECEIVED ===");
        Console.WriteLine($"InvoiceId   : {msg.InvoiceId}");
        Console.WriteLine($"CustomerId  : {msg.CustomerId}");
        Console.WriteLine($"Amount      : {msg.Amount}");
        Console.WriteLine($"Currency    : {msg.Currency}");
        Console.WriteLine($"InvoiceDate : {msg.InvoiceDate:O}");
        Console.WriteLine($"Description : {msg.Description}");
        Console.WriteLine();

        return Task.CompletedTask;
    }
}