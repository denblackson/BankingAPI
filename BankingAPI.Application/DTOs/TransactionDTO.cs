using System.ComponentModel.DataAnnotations;
using BankingAPI.Domain.Enums;

namespace BankingAPI.Application.DTOs;

public record TransactionDTO(
    int Id,
    [Required] TransactionType TransactionType,
    [Required] TransactionCategory TransactionCategory,
    [Required, Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    decimal Amount,
    [Required] DateTime Timestamp,
    int? SourceAccountId,
    int? DestinationAccountId,
    [StringLength(250)] string Description
);