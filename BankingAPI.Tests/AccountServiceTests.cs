using System.Linq.Expressions;
using BankingAPI.Application.Conversions;
using BankingAPI.Application.DTOs;
using BankingAPI.Application.Interfaces.Repositories;
using BankingAPI.Application.Services;
using BankingAPI.Domain.Entities;
using Moq;

namespace BankingAPI.Tests;

public class AccountServiceTests
{
    private readonly Mock<IAccountRepository> _accountRepoMock;
    private readonly AccountServices _accountService;

    public AccountServiceTests()
    {
        _accountRepoMock = new Mock<IAccountRepository>();
        _accountService = new AccountServices(_accountRepoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateAccount_WhenAccountDoesNotExist()
    {
        // Arrange
        var accountDto = new CreateAccountDTO(
            "ENG12345678",
            "Test User"
        );

        _accountRepoMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<Account, bool>>>()))
            .ReturnsAsync((Account)null);
        _accountRepoMock.Setup(r => r.CreateAsync(It.IsAny<Account>()))
            .ReturnsAsync((Account a) =>
            {
                a.Id = 1;
                return a;
            });

        // Act
        var result = await _accountService.CreateAsync(AccountConversion.ToEntity(accountDto));

        // Assert
        Assert.True(result.Flag);
        Assert.Equal("Account ENG12345678 has been added successfully", result.Message);
        _accountRepoMock.Verify(r => r.CreateAsync(It.IsAny<Account>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenAccountAlreadyExists()
    {
        // Arrange
        var existingAccount = new Account { Id = 1, AccountNumber = "ENG12345678" };
        _accountRepoMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<Account, bool>>>()))
            .ReturnsAsync(existingAccount);

        var accountDto = new CreateAccountDTO(
            "ENG12345678",
            "Test User"
        );
        // Act
        var result = await _accountService.CreateAsync(AccountConversion.ToEntity(accountDto));

        // Assert
        Assert.False(result.Flag);
        Assert.Contains("already exists", result.Message);
        _accountRepoMock.Verify(r => r.CreateAsync(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAccount_WhenAccountExists()
    {
        // Arrange
        var account = new Account { Id = 1, AccountNumber = "ENG12345678", OwnerName = "Old Name" };
        _accountRepoMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<Account, bool>>>()))
            .ReturnsAsync(account);
        _accountRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Account>())).Returns(Task.CompletedTask);

        account.OwnerName = "New Name";

        // Act
        var result = await _accountService.UpdateAsync(account);

        // Assert
        Assert.True(result.Flag);
        _accountRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Account>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldFail_WhenAccountDoesNotExist()
    {
        // Arrange
        _accountRepoMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<Account, bool>>>()))
            .ReturnsAsync((Account)null);

        var account = new Account { Id = 1, AccountNumber = "ENG12345678", OwnerName = "Test" };

        // Act
        var result = await _accountService.UpdateAsync(account);

        // Assert
        Assert.False(result.Flag);
        Assert.Contains("does not exist", result.Message);
        _accountRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteAccount_WhenAccountExists()
    {
        // Arrange
        var account = new Account { Id = 1, AccountNumber = "ENG12345678" };
        _accountRepoMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<Account, bool>>>()))
            .ReturnsAsync(account);
        _accountRepoMock.Setup(r => r.DeleteAsync(It.IsAny<Account>())).Returns(Task.CompletedTask);

        // Act
        var result = await _accountService.DeleteAsync(account.Id);

        // Assert
        Assert.True(result.Flag);
        Assert.Equal("Account 1 deleted successfully", result.Message);
        _accountRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Account>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldFail_WhenAccountDoesNotExist()
    {
        // Arrange
        _accountRepoMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<Account, bool>>>()))
            .ReturnsAsync((Account)null);

        // Act
        var result = await _accountService.DeleteAsync(1);

        // Assert
        Assert.False(result.Flag);
        _accountRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenAccountNumberIsEmpty()
    {
        // Arrange
        var accountDto = new CreateAccountDTO("", "Test User");

        // Act
        var result = await _accountService.CreateAsync(AccountConversion.ToEntity(accountDto));

        // Assert
        Assert.False(result.Flag);
    }
}