using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace BankingAPI.Domain.Entities;

public class Account
{
  public int Id { get; set; }
    public string AccountNumber { get; set; }
    public string OwnerName { get; set; }
    public decimal Balance { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsActive { get; set; } = true;
    public ICollection<Transaction> Transactions { get; set; }
}