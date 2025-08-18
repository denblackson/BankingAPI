using System.Linq.Expressions;
using BankingAPI.Application.Interfaces;
using BankingAPI.Application.Interfaces.Repositories;
using BankingAPI.Application.Services;
using BankingAPI.Domain.Entities;
using BankingAPI.Domain.Enums;
using Moq;

namespace BankingAPI.Tests;

public class TransactionServiceTests
{
    private readonly Mock<ITransactionRepository> _transactionRepoMock;
    private readonly Mock<IAccountRepository> _accountRepoMock;
    private readonly TransactionServices _transactionService;

    public TransactionServiceTests()
    {
        _transactionRepoMock = new Mock<ITransactionRepository>();
        _accountRepoMock = new Mock<IAccountRepository>();
        _transactionService = new TransactionServices(_transactionRepoMock.Object, _accountRepoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateIncomeTransaction_WhenDestinationAccountExists()
    {
        // Arrange
        var transaction = new Transaction
        {
            TransactionType = TransactionType.Income,
            Amount = 50,
            DestinationAccount = new Account { AccountNumber = "FR12345678" }
        };

        var destAccount = new Account
        {
            Id = 1,
            AccountNumber = "FR12345678",
            Balance = 100
        };

        _accountRepoMock.Setup(a => a.GetByAsync(It.IsAny<Expression<Func<Account, bool>>>()))
            .ReturnsAsync(destAccount);
        _transactionRepoMock.Setup(t => t.CreateAsync(It.IsAny<Transaction>()))
            .ReturnsAsync((Transaction tr) =>
            {
                tr.Id = 1;
                return tr;
            });

        // Act
        var result =
            await _transactionService.DepositAsync(transaction.DestinationAccount.AccountNumber, transaction.Amount,
                "");

        // Assert
        Assert.True(result.Flag);
        Assert.Equal(150, destAccount.Balance);
        _transactionRepoMock.Verify(t => t.CreateAsync(It.IsAny<Transaction>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldWithdraw_WhenSourceAccountExists()
    {
        // Arrange
        var source = new Account { Id = 1, AccountNumber = "FR12345678", Balance = 100 };
        _accountRepoMock.Setup(r => r.GetByAsync(a => a.AccountNumber == "FR12345678"))
            .ReturnsAsync(source);

        var transaction = new Transaction
        {
            TransactionType = TransactionType.Outcome,
            Amount = 50,
            SourceAccount = new Account { AccountNumber = "FR12345678" }
        };

        _transactionRepoMock.Setup(r => r.CreateAsync(It.IsAny<Transaction>()))
            .ReturnsAsync((Transaction t) =>
            {
                t.Id = 1;
                return t;
            });

        // Act
        var result =
            await _transactionService.WithdrawAsync(transaction.SourceAccount.AccountNumber, transaction.Amount);

        // Assert
        Assert.True(result.Flag);
        Assert.Equal(50, source.Balance);
        _transactionRepoMock.Verify(r => r.CreateAsync(It.IsAny<Transaction>()), Times.Once);
        _accountRepoMock.Verify(r => r.UpdateAsync(source), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenInsufficientFunds()
    {
        // Arrange
        var source = new Account { Id = 1, AccountNumber = "FR12345678", Balance = 30 };
        _accountRepoMock.Setup(r => r.GetByAsync(a => a.AccountNumber == "FR12345678"))
            .ReturnsAsync(source);

        var transaction = new Transaction
        {
            TransactionType = TransactionType.Outcome,
            Amount = 50,
            SourceAccount = new Account { AccountNumber = "FR12345678" }
        };

        // Act
        var result =
            await _transactionService.WithdrawAsync(source.AccountNumber, transaction.Amount, transaction.Description);

        // Assert
        Assert.False(result.Flag);
        _transactionRepoMock.Verify(r => r.CreateAsync(It.IsAny<Transaction>()), Times.Never);
        _accountRepoMock.Verify(r => r.UpdateAsync(source), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldTransfer_WhenSourceAndDestinationExist()
    {
        // Arrange
        var source = new Account { Id = 1, AccountNumber = "DEU11111111", Balance = 200 };
        var destination = new Account { Id = 2, AccountNumber = "FR12345678", Balance = 100 };

        _accountRepoMock.Setup(r => r.GetByAsync(a => a.AccountNumber == "DEU11111111")).ReturnsAsync(source);
        _accountRepoMock.Setup(r => r.GetByAsync(a => a.AccountNumber == "FR12345678")).ReturnsAsync(destination);

        var transaction = new Transaction
        {
            TransactionType = TransactionType.Outcome, // для списання
            Amount = 50,
            SourceAccount = new Account { AccountNumber = "DEU11111111" },
            DestinationAccount = new Account { AccountNumber = "FR12345678" }
        };

        _transactionRepoMock.Setup(r => r.CreateAsync(It.IsAny<Transaction>()))
            .ReturnsAsync((Transaction t) =>
            {
                t.Id = 1;
                return t;
            });

        // Act
        var result = await _transactionService.TransferAsync(transaction.SourceAccount.AccountNumber,
            transaction.DestinationAccount.AccountNumber, transaction.Amount);

        // Assert
        Assert.True(result.Flag);
        Assert.Equal(150, source.Balance);
        Assert.Equal(150, destination.Balance);
        _transactionRepoMock.Verify(r => r.CreateAsync(It.IsAny<Transaction>()), Times.Once);
        _accountRepoMock.Verify(r => r.UpdateAsync(source), Times.Once);
        _accountRepoMock.Verify(r => r.UpdateAsync(destination), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldFailTransfer_WhenInsufficientFunds()
    {
        // Arrange
        var source = new Account { Id = 1, AccountNumber = "DEU11111111", Balance = 30 };
        var destination = new Account { Id = 2, AccountNumber = "FR12345678", Balance = 100 };

        _accountRepoMock.Setup(r => r.GetByAsync(a => a.AccountNumber == "DEU11111111")).ReturnsAsync(source);
        _accountRepoMock.Setup(r => r.GetByAsync(a => a.AccountNumber == "FR12345678")).ReturnsAsync(destination);

        var transaction = new Transaction
        {
            TransactionType = TransactionType.Outcome,
            Amount = 50,
            SourceAccount = new Account { AccountNumber = "DEU11111111" },
            DestinationAccount = new Account { AccountNumber = "FR12345678" }
        };

        // Act
        var result = await _transactionService.TransferAsync(transaction.SourceAccount.AccountNumber,
            transaction.DestinationAccount.AccountNumber, transaction.Amount);

        // Assert
        Assert.False(result.Flag);
        _transactionRepoMock.Verify(r => r.CreateAsync(It.IsAny<Transaction>()), Times.Never);
        _accountRepoMock.Verify(r => r.UpdateAsync(source), Times.Never);
        _accountRepoMock.Verify(r => r.UpdateAsync(destination), Times.Never);
    }
}