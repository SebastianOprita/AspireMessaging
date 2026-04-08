using System.ComponentModel.DataAnnotations;

namespace AspireMessaging.Producer.Dtos;

public class NotificationDto
{
    [Required] public Guid NotificationId { get; init; }
    [Required] public Guid InvoiceId { get; init; }
    [Required] public Guid PaymentId { get; init; }
    public DateTime? PaymentDate { get; init; } = null;
    public string? Description { get; init; }
}
