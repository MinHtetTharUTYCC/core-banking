namespace CoreBanking.Application.Common.Extensions;

public static class StringMaskExtensions
{
    public static string MaskAccountNumber(this string accountNumber, int visibleDigits = 4)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            return string.Empty;

        if (accountNumber.Length <= visibleDigits)
            return new string('*', accountNumber.Length);

        var maskedLength = accountNumber.Length - visibleDigits;
        var visiblePart = accountNumber[^visibleDigits..];

        return new string('*', maskedLength) + visiblePart;
    }
}
