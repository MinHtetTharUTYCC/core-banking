namespace CoreBanking.Application.Accounts.Commands;

public enum DepositFailureReason
{
    None,
    AccountNotFound,
    Unauthorized,
}