namespace CoreBanking.Application.Models;

public class WelcomeEmailModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string BankName { get; set; } = "CoreBanking";
    public DateTime RegistrationDate { get; set; }
}

public class PasswordResetEmailModel
{
    public string FullName { get; set; } = string.Empty;
    public string ResetLink { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 30;
}

public class LoanApprovedEmailModel
{
    public string FullName { get; set; } = string.Empty;
    public string LoanNumber { get; set; } = string.Empty;
    public decimal ApprovedAmount { get; set; }
    public string Currency { get; set; } = "USD";
}

public class TransactionEmailModelBase
{
    public string Email { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime TransactionDate { get; set; }
    public decimal NewBalance { get; set; }
}

public class  TransactionSentEmailModel : TransactionEmailModelBase
{
    public string RecipientName { get; set; } = string.Empty;
    public string RecipientAccountNumberMasked { get; set; } = string.Empty;
}

public class TransactionReceivedEmailModel: TransactionEmailModelBase
{
    public string SenderName { get; set; } = string.Empty;
    public string SenderAccountNumberMasked { get; set; } = string.Empty;
}

public class DepositEmailModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime TransactionDate { get; set; }
    public decimal NewBalance { get; set; }
}

public class WithdrawEmailModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime TransactionDate { get; set; }
    public decimal NewBalance { get; set; }
}
