using MediatR;
using Microsoft.AspNetCore.Mvc;
using CoreBanking.Application.Accounts.Commands;
using CoreBanking.Application.Accounts.Queries;
using CoreBanking.Domain.Enums;

namespace CoreBanking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public AccountsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccountRequest request)
    {
        var command = new CreateAccountCommand
        {
            OwnerName = request.OwnerName,
            OwnerEmail = request.OwnerEmail,
            AccountType = request.AccountType,
            Currency = request.Currency
        };

        var id = await _mediator.Send(command);

        // CreatedAtAction overloads:
        // 1. CreatedAtAction(actionName, value)
        // 2. CreatedAtAction(actionName, routeValues, value)
        // 3. CreatedAtAction(actionName, controllerName, routeValues, value)
        // Returns 201 Created + Location header pointing to GetById + response body { id }
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetAccountByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound();
        
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllAccountsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpPost("{id:guid}/deposit")]
    public async Task<IActionResult> Deposit(Guid id, [FromBody] DepositRequest request)
    {
        var command = new DepositCommand { AccountId = id, Amount = request.Amount };
        var result = await _mediator.Send(command);
        
        if (!result)
            return NotFound();
        
        return Ok(new { message = "Deposit successful" });
    }
    
    [HttpPost("{id:guid}/withdraw")]
    public async Task<IActionResult> Withdraw(Guid id, [FromBody] WithdrawRequest request)
    {
        var command = new WithdrawCommand { AccountId = id, Amount = request.Amount };
        var result = await _mediator.Send(command);
        
        if (!result)
            return NotFound();
        
        return Ok(new { message = "Withdrawal successful" });
    }
    
    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
    {
        var command = new TransferCommand
        {
            FromAccountId = request.FromAccountId,
            ToAccountId = request.ToAccountId,
            Amount = request.Amount
        };
        
        var result = await _mediator.Send(command);
        
        if (!result)
            return BadRequest(new { message = "Transfer failed" });
        
        return Ok(new { message = "Transfer successful" });
    }
}

public record CreateAccountRequest(
    string OwnerName,
    string OwnerEmail,
    AccountType AccountType,
    Currency Currency);

public record CreateAccountRequestCopy(
    string OwnerName,
    string OwnerEmail,
    AccountType AccountType,
    Currency Currency);

public record DepositRequest(decimal Amount);
public record WithdrawRequest(decimal Amount);
public record TransferRequest(Guid FromAccountId, Guid ToAccountId, decimal Amount);
