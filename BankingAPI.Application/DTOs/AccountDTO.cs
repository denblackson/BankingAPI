using System.ComponentModel.DataAnnotations;

namespace BankingAPI.Application.DTOs;

public record AccountDTO(
    int Id,
    [Required, StringLength(20, MinimumLength = 10)]
    string AccountNumber,
    [Required, StringLength(100)] string OwnerName,
    [Range(0, double.MaxValue)] decimal Balance,
    [Required] DateTime CreatedAt,
    bool IsActive
);