using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoreBanking.Application.Transactions.Queries;

using CoreBanking.Api.Models;

namespace CoreBanking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TransactionQueryParams parameters)
    {
        var query = new GetAllTransactionsQuery
        {
            SortBy = parameters.SortBy,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetTransactionByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("~/api/accounts/{accountId:guid}/transactions")]
    public async Task<IActionResult> GetByAccountId(Guid accountId, [FromQuery] TransactionQueryParams parameters)
    {
        var query = new GetTransactionsByAccountIdQuery
        {
            AccountId = accountId,
            SortBy = parameters.SortBy,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
