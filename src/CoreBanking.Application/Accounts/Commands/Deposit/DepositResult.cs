namespace CoreBanking.Application.Accounts.Commands;

public record DepositResult(bool Success, DepositFailureReason FailureReason);