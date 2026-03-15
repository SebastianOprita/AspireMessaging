namespace AspireMessaging.Contracts;

public record PaymentSubmitted(
    Guid PaymentId,
    Guid InvoiceId,
    decimal Amount,
    DateTime PaymentDate,
    string Currency,
    string? Method
);
