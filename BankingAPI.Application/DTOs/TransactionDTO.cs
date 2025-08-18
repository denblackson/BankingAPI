using System.ComponentModel.DataAnnotations;
using BankingAPI.Domain.Enums;

namespace BankingAPI.Application.DTOs;

public record TransactionDTO(
    int Id,
    [Required] TransactionType TransactionType,
    [Required] TransactionCategory TransactionCategory,
    [Required, Range(0.01, double.MaxValue)]
    decimal Amount,
    [Required] DateTime Timestamp,
    [StringLength(250)] string Description
);