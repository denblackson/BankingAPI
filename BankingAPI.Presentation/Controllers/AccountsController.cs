using BankingAPI.Application.Conversions;
using BankingAPI.Application.DTOs;
using BankingAPI.Application.Interfaces;
using BankingAPI.Application.Interfaces.Repositories;
using BankingAPI.Domain.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BankingAPI.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController(IAccountService accountService) : ControllerBase
{
    [HttpGet("GetAllAccounts")]
    public async Task<ActionResult<IEnumerable<AccountDTO>>> GetAccounts()
    {
        var accounts = await accountService.GetAllAsync();
        if (!accounts.Any())
            return NotFound("No Accounts found in the database");

        // конвертація в DTO
        var (_, list) = AccountConversion.FromEntity(null!, accounts);
        return Ok(list);
    }

    [HttpGet("GetAccountById/{id:int}")]
    public async Task<ActionResult<AccountDTO>> GetAccountById(int id)
    {
        var account = await accountService.GetByAsync(a => a.Id == id);
        if (account is null)
            return NotFound($"Account with id {id} not found");

        var (dto, _) = AccountConversion.FromEntity(account, null);
        return Ok(dto);
    }

    [HttpPost("CreateAccount")]
    public async Task<ActionResult<Response>> CreateAccount([FromBody] AccountDTO accountDto)
    {
        var entity = AccountConversion.ToEntity(accountDto);
        var response = await accountService.CreateAsync(entity);
        return response.Flag ? Ok(response) : BadRequest(response);
    }

    [HttpPut("UpdateAccount")]
    public async Task<ActionResult<Response>> UpdateAccount([FromBody] AccountDTO accountDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = AccountConversion.ToEntity(accountDto);
        var response = await accountService.UpdateAsync(entity);
        return response.Flag ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("DeleteAccountById/{id:int}")]
    public async Task<ActionResult<Response>> DeleteAccountById(int id)
    {
        var response = await accountService.DeleteAsync(id);
        return response.Flag ? Ok(response) : BadRequest(response);
    }
}