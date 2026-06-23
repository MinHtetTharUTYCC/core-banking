using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoreBanking.Api.Models;
using CoreBanking.Application.Accounts.Commands;
using CoreBanking.Application.Accounts.Queries;

namespace CoreBanking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
            OwnerEmail = User.FindFirstValue(ClaimTypes.Email)!,
            AccountType = request.AccountType,
            Currency = request.Currency
        };

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAccountById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAccountById(Guid id)
    {
        var query = new GetAccountByIdQuery
        {
            Id = id,
            OwnerEmail = User.FindFirstValue(ClaimTypes.Email)!
        };

        var result = await _mediator.Send(query);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAccounts()
    {
        var query = new GetAllAccountsQuery
        {
            OwnerEmail = User.FindFirstValue(ClaimTypes.Email)!
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("{id:guid}/deposit")]
    public async Task<IActionResult> Deposit(Guid id, [FromBody] DepositRequest request, CancellationToken ct)
    {
        var command = new DepositCommand
        {
            AccountId = id,
            Amount = request.Amount,
            OwnerEmail = User.FindFirstValue(ClaimTypes.Email)!,
            IdempotencyKey = request.IdempotencyKey
        };

        await _mediator.Send(command, ct);

        return Ok(new { message = "Deposit successful" });
    }

    [HttpPost("{id:guid}/withdraw")]
    public async Task<IActionResult> Withdraw(Guid id, [FromBody] WithdrawRequest request, CancellationToken ct)
    {
        var command = new WithdrawCommand
        {
            AccountId = id,
            Amount = request.Amount,
            OwnerEmail = User.FindFirstValue(ClaimTypes.Email)!,
            IdempotencyKey = request.IdempotencyKey
        };

        await _mediator.Send(command, ct);

        return Ok(new { message = "Withdrawal successful" });
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request, CancellationToken ct)
    {
        var command = new TransferCommand
        {
            FromAccountId = request.FromAccountId,
            ToAccountId = request.ToAccountId,
            Amount = request.Amount,
            OwnerEmail = User.FindFirstValue(ClaimTypes.Email)!,
            IdempotencyKey = request.IdempotencyKey
        };

        await _mediator.Send(command, ct);

        return Ok(new { message = "Transfer successful" });
    }
}
