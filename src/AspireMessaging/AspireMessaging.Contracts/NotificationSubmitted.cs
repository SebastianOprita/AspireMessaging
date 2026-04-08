using System;

namespace AspireMessaging.Contracts;

public record NotificationSubmitted(
    Guid NotificationId,
    Guid InvoiceId,
    Guid PaymentId,
    DateTime? PaymentDate,
    string? Description
);