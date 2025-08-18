using BankingAPI.Application.DTOs;
using BankingAPI.Domain.Entities;

namespace BankingAPI.Application.Conversions;

public class TransactionConversion
{
    public static Transaction ToEntity(TransactionDTO transactionDTO) => new()
    {
        Id = transactionDTO.Id,
        TransactionType = transactionDTO.TransactionType,
        TransactionCategory = transactionDTO.TransactionCategory,
        Amount = transactionDTO.Amount,
        Timestamp = transactionDTO.Timestamp,
        SourceAccountId = transactionDTO.SourceAccountId,
        DestinationAccountId = transactionDTO.DestinationAccountId,
        Description = transactionDTO.Description
    };

    public static (TransactionDTO?, IEnumerable<TransactionDTO>?) FromEntity(Transaction transaction,
        IEnumerable<System.Transactions.Transaction>? transactions)
    {
        if (transaction is not null || transactions is null)
        {
            var _singleTransaction = new TransactionDTO(transaction!.Id, transaction.TransactionType,
                transaction.TransactionCategory,
                transaction.Amount, transaction.Timestamp, transaction.SourceAccountId,
                transaction.DestinationAccountId, transaction.Description);
            return (_singleTransaction, null);
        }

        if (transaction is null || transactions is not null)
        {
            var _transactions = transactions.Select(t => new TransactionDTO(transaction!.Id,
                transaction.TransactionType,
                transaction.TransactionCategory,
                transaction.Amount, transaction.Timestamp, transaction.SourceAccountId,
                transaction.DestinationAccountId, transaction.Description)).ToList();
            return (null, _transactions);
        }

        return (null, null);
    }
}