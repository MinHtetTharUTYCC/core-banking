using MediatR;
using Microsoft.AspNetCore.Mvc;
using CoreBanking.Api.Models;
using CoreBanking.Application.Loans.Commands;
using CoreBanking.Application.Loans.Queries;

namespace CoreBanking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly IMediator _mediator;

    public LoansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Apply([FromBody] ApplyLoanRequest request)
    {
        var command = new ApplyLoanCommand
        {
            AccountId = request.AccountId,
            LoanType = request.LoanType,
            PrincipalAmount = request.PrincipalAmount,
            Currency = request.Currency,
            InterestRate = request.InterestRate,
            TermMonths = request.TermMonths
        };

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetLoanByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] LoanQueryParams parameters)
    {
        var query = new GetAllLoansQuery
        {
            SortBy = parameters.SortBy,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("~/api/accounts/{accountId:guid}/loans")]
    public async Task<IActionResult> GetByAccountId(Guid accountId, [FromQuery] LoanQueryParams parameters)
    {
        var query = new GetLoansByAccountIdQuery
        {
            AccountId = accountId,
            SortBy = parameters.SortBy,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id)
    {
        var command = new ApproveLoanCommand { LoanId = id };
        var result = await _mediator.Send(command);

        if (!result)
            return NotFound();

        return Ok(new { message = "Loan approved successfully" });
    }

    [HttpPost("{id:guid}/disburse")]
    public async Task<IActionResult> Disburse(Guid id)
    {
        var command = new DisburseLoanCommand { LoanId = id };
        var result = await _mediator.Send(command);

        if (!result)
            return NotFound();

        return Ok(new { message = "Loan disbursed successfully" });
    }

    [HttpPost("{id:guid}/payments")]
    public async Task<IActionResult> MakePayment(Guid id, [FromBody] MakeLoanPaymentRequest request)
    {
        var command = new MakeLoanPaymentCommand
        {
            LoanId = id,
            Amount = request.Amount
        };

        var result = await _mediator.Send(command);

        if (!result)
            return NotFound();

        return Ok(new { message = "Payment successful" });
    }
}
