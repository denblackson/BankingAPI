using BankingAPI.Application.DTOs;
using BankingAPI.Application.Interfaces;
using BankingAPI.Domain.Entities;
using BankingAPI.Domain.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BankingAPI.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController(ITransactionService transactionService) : ControllerBase
{
    [HttpGet("GetAllTransactions")]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetAllTransactions()
    {
        var transactions = await transactionService.GetAllAsync();

        if (!transactions.Any())
            return NotFound("No transactions found");

        return Ok(transactions);
    }

    [HttpGet("GetTransactionById/{id:int}")]
    public async Task<ActionResult<Transaction>> GetTransactionById(int id)
    {
        var transaction = await transactionService.GetByAsync(t => t.Id == id);

        if (transaction is null)
            return NotFound($"Transaction {id} not found");

        return Ok(transaction);
    }


    [HttpDelete("Delete/{id:int}")]
    public async Task<ActionResult<Response>> DeleteTransaction(int id)
    {
        var result = await transactionService.DeleteAsync(id);

        return result.Flag is true ? Ok(result) : NotFound(result);
    }

    [HttpPost("Deposit")]
    public async Task<ActionResult<Response>> Deposit([FromBody] DepositRequest request)
    {
        var result = await transactionService.DepositAsync(request.AccountNumber, request.Amount, request.Description);
        return result.Flag ? Ok(result) : BadRequest(result);
    }

    [HttpPost("Withdraw")]
    public async Task<ActionResult<Response>> Withdraw([FromBody] WithdrawRequest request)
    {
        var result = await transactionService.WithdrawAsync(request.AccountNumber, request.Amount, request.Description);
        return result.Flag ? Ok(result) : BadRequest(result);
    }

    [HttpPost("Transfer")]
    public async Task<ActionResult<Response>> Transfer([FromBody] TransferRequest request)
    {
        var result = await transactionService.TransferAsync(request.SourceAccountNumber,
            request.DestinationAccountNumber, request.Amount, request.Description);
        return result.Flag ? Ok(result) : BadRequest(result);
    }


    // [HttpPut("Update")]
    // public async Task<ActionResult<Response>> UpdateTransaction([FromBody] Transaction transaction)
    // {
    //     if (!ModelState.IsValid)
    //         return BadRequest(ModelState);
    //
    //     var result = await transactionService.UpdateAsync(transaction);
    //
    //     return result.Flag is true ? Ok(result) : BadRequest(result);
    // }
}