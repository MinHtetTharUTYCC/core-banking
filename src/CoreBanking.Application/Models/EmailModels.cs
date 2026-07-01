namespace CoreBanking.Application.Models;

public class WelcomeEmailModel
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string BankName { get; set; } = "CoreBanking";
    public required DateTime RegistrationDate { get; set; }
}

public class PasswordResetEmailModel
{
    public required string FullName { get; set; }
    public required string ResetLink { get; set; }
    public int ExpiryMinutes { get; set; } = 30;
}

public class LoanApprovedEmailModel
{
    public required string FullName { get; set; }
    public required string LoanNumber { get; set; }
    public required decimal ApprovedAmount { get; set; }
    public string Currency { get; set; } = "USD";
}

public class TransactionEmailModelBase
{
    public required string Email { get; set; }
    public required string FullName { get; set; }
    public string BankName { get; set; } = "CoreBanking";
    public required decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public required DateTime TransactionDate { get; set; }
    public required decimal NewBalance { get; set; }
}

public class  TransactionSentEmailModel : TransactionEmailModelBase
{
    public required string RecipientName { get; set; }
    public required string RecipientAccountNumberMasked { get; set; }
    
    public required string TransferReferenceNumber { get; set; }
}

public class TransactionReceivedEmailModel: TransactionEmailModelBase
{
    public required string SenderName { get; set; }
    public required string SenderAccountNumberMasked { get; set; }
    
    public required string TransferReferenceNumber { get; set; }

}

public class DepositEmailModel
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string BankName { get; set; } = "CoreBanking";
    public required decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public required DateTime TransactionDate { get; set; }
    public required decimal NewBalance { get; set; }
}

public class WithdrawEmailModel
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string BankName { get; set; } = "CoreBanking";
    public required decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public required DateTime TransactionDate { get; set; }
    public required decimal NewBalance { get; set; }
}

public class LoggedInEmailModel
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string BankName { get; set; } = "CoreBanking";
    public required string Device { get; set; }
    public required string Browser { get; set; }
    public required string OperatingSystem { get; set; }
    public required string IpAddress { get; set; }
    public required DateTime LoginDateTime { get; set; } = DateTime.UtcNow; 
    
}
