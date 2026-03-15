using System.ComponentModel.DataAnnotations;

namespace AspireMessaging.Producer;

public class InvoiceDto
{
    [Required] public string CustomerId { get; init; } = null!;
    [Range(0.01, double.MaxValue)] public decimal Amount { get; init; }
    public DateTime? InvoiceDate { get; init; } = null;
    [Required] public string Currency { get; init; } = "USD";
    public string? Description { get; init; }
}