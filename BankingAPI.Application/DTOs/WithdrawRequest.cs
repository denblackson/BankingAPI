namespace BankingAPI.Application.DTOs;

public record WithdrawRequest(string AccountNumber, decimal Amount, string Description);