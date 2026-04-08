using AspireMessaging.Contracts;
using MassTransit;

namespace AspireMessaging.Consumer;

public class EmailNotificationConsumer : IConsumer<NotificationSubmitted>
{
    public Task Consume(ConsumeContext<NotificationSubmitted> context)
    {
        var msg = context.Message;

        Console.WriteLine("=== EMAIL NOTIFICATION RECEIVED ===");
        Console.WriteLine($"NotificationId : {msg.NotificationId}");
        Console.WriteLine($"InvoiceId      : {msg.InvoiceId}");
        Console.WriteLine($"PaymentId      : {msg.PaymentId}");
        Console.WriteLine($"PaymentDate    : {msg.PaymentDate:O}");
        Console.WriteLine($"Description    : {msg.Description}");
        Console.WriteLine();

        return Task.CompletedTask;
    }
}
