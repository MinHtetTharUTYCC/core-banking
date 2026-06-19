using System.ComponentModel.DataAnnotations;
using CoreBanking.Domain.Enums;

namespace CoreBanking.Api.Models;

public class CreateAccountRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string OwnerName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string OwnerEmail { get; init; } = string.Empty;

    [Required]
    public AccountType AccountType { get; init; }

    [Required]
    public Currency Currency { get; init; }
}

public class DepositRequest
{
    [Required]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; init; }
}

public class WithdrawRequest
{
    [Required]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; init; }
}

public class TransferRequest
{
    [Required]
    public Guid FromAccountId { get; init; }

    [Required]
    public Guid ToAccountId { get; init; }

    [Required]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; init; }
}
