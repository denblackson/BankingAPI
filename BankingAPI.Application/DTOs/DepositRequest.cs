namespace BankingAPI.Application.DTOs;

public record DepositRequest(string AccountNumber, decimal Amount, string Description);