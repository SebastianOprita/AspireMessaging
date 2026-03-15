using AspireMessaging.Contracts;
using MassTransit;

namespace AspireMessaging.Consumer;

public class PaymentSubmittedConsumer : IConsumer<PaymentSubmitted>
{
    public Task Consume(ConsumeContext<PaymentSubmitted> context)
    {
        var msg = context.Message;

        Console.WriteLine("=== PAYMENT RECEIVED ===");
        Console.WriteLine($"PaymentId   : {msg.PaymentId}");
        Console.WriteLine($"InvoiceId   : {msg.InvoiceId}");
        Console.WriteLine($"Amount      : {msg.Amount}");
        Console.WriteLine($"Currency    : {msg.Currency}");
        Console.WriteLine($"PaymentDate : {msg.PaymentDate:O}");
        Console.WriteLine($"Method      : {msg.Method}");
        Console.WriteLine();

        return Task.CompletedTask;
    }
}