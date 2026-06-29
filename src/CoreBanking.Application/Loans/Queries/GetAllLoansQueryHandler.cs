using MediatR;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Application.Common.Models;
using CoreBanking.Domain.Entities;

namespace CoreBanking.Application.Loans.Queries;

public class GetAllLoansQueryHandler(ILoanRepository repository) : IRequestHandler<GetAllLoansQuery, PaginatedResult<LoanDto>>
{
    public async Task<PaginatedResult<LoanDto>> Handle(GetAllLoansQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetAllAsync(request.SortBy, request.Page, request.PageSize);

        return new PaginatedResult<LoanDto>
        {
            Items = result.Items.Select(MapToDto).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }

    internal static LoanDto MapToDto(Loan loan)
    {
        return new LoanDto
        {
            Id = loan.Id,
            LoanNumber = loan.LoanNumber.Value,
            AccountId = loan.AccountId,
            LoanType = loan.LoanType.ToString(),
            PrincipalAmount = loan.PrincipalAmount.Amount,
            Currency = loan.PrincipalAmount.Currency.ToString(),
            OutstandingBalance = loan.OutstandingBalance.Amount,
            InterestRate = loan.InterestRate,
            TermMonths = loan.TermMonths,
            MonthlyPayment = loan.MonthlyPayment,
            Status = loan.Status.ToString(),
            CreatedAt = loan.CreatedAt,
            ApprovedAt = loan.ApprovedAt,
            DisbursedAt = loan.DisbursedAt,
            ClosedAt = loan.ClosedAt,
            RejectionReason = loan.RejectionReason
        };
    }
}
