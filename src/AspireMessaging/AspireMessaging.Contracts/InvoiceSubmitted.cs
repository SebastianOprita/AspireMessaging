namespace AspireMessaging.Contracts;

public record InvoiceSubmitted(
    Guid InvoiceId,
    string CustomerId,
    decimal Amount,
    DateTime InvoiceDate,
    string Currency,
    string? Description
);