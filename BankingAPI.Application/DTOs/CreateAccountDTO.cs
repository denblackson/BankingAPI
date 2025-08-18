using System.ComponentModel.DataAnnotations;

namespace BankingAPI.Application.DTOs;

public record CreateAccountDTO(
    [Required, StringLength(20, MinimumLength = 10)]
    string AccountNumber,
    [Required, StringLength(100)]
    string OwnerName
);