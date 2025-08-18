using BankingAPI.Domain.Enums;

namespace BankingAPI.Domain.Entities;

public class Transaction
{
    public int Id { get; set; }
    public TransactionType TransactionType { get; set; }
    public TransactionCategory TransactionCategory { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int? SourceAccountId { get; set; }
    public Account SourceAccount { get; set; }
    public int? DestinationAccountId { get; set; }
    public Account DestinationAccount { get; set; }
    public string Description { get; set; }
}