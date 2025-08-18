using BankingAPI.Domain.Enums;

namespace BankingAPI.Domain.Entities;

// public class Transaction
// {
//     public int Id { get; set; }
//     public TransactionType TransactionType { get; set; }
//     public TransactionCategory TransactionCategory { get; set; }
//     public decimal Amount { get; set; }
//     public DateTime Timestamp { get; set; } = DateTime.UtcNow;
//     public int? SourceAccountId { get; set; }
//     public Account? SourceAccount { get; set; }
//     public int? DestinationAccountId { get; set; }
//     public Account? DestinationAccount { get; set; }
//     public string Description { get; set; }
// }


public class Transaction
{
    public int Id { get; set; }
    public TransactionType TransactionType { get; set; }
    public TransactionCategory TransactionCategory { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;

    // IDs зберігаємо для внутрішніх зв’язків, якщо треба
    public int? SourceAccountId { get; set; }
    public Account? SourceAccount { get; set; }

    public int? DestinationAccountId { get; set; }
    public Account? DestinationAccount { get; set; }

    // Нові властивості для роботи по AccountNumber
  //  public string? SourceAccountNumber { get; set; }
   // public string? DestinationAccountNumber { get; set; }

    public string Description { get; set; }
}
