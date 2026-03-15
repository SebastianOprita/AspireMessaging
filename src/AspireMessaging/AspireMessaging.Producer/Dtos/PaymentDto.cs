using System.ComponentModel.DataAnnotations;

namespace AspireMessaging.Producer;

public class PaymentDto
{
    [Required] public Guid InvoiceId { get; init; }
    [Range(0.01, double.MaxValue)] public decimal Amount { get; init; }
    public DateTime? PaymentDate { get; init; } = null;
    [Required] public string Currency { get; init; } = "USD";
    public string? Method { get; init; }
}