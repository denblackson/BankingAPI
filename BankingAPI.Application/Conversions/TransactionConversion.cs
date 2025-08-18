using BankingAPI.Application.DTOs;
using BankingAPI.Domain.Entities;

namespace BankingAPI.Application.Conversions;

public class TransactionConversion
{
    public static Transaction ToEntity(TransactionDTO dto) => new()
    {
        Id = dto.Id,
        TransactionType = dto.TransactionType,
        TransactionCategory = dto.TransactionCategory,
        Amount = dto.Amount,
        Timestamp = dto.Timestamp,
        Description = dto.Description
    };

    public static (TransactionDTO?, IEnumerable<TransactionDTO>?) FromEntity(Transaction transaction,
        IEnumerable<Transaction>? transactions)
    {
        if (transaction != null)
        {
            var singleDto = new TransactionDTO(
                transaction.Id,
                transaction.TransactionType,
                transaction.TransactionCategory,
                transaction.Amount,
                transaction.Timestamp,
                transaction.Description
            );
            return (singleDto, null);
        }

        if (transactions != null)
        {
            var dtos = transactions.Select(t => new TransactionDTO(
                t.Id,
                t.TransactionType,
                t.TransactionCategory,
                t.Amount,
                t.Timestamp,
                t.Description
            )).ToList();
            return (null, dtos);
        }

        return (null, null);
    }
}