namespace BankingAPI.Application.DTOs;

public record TransferRequest(string SourceAccountNumber, string DestinationAccountNumber, decimal Amount, string Description);