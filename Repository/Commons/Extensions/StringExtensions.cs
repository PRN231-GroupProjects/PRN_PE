using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Repository.Commons.Extensions;

public static class StringExtensions
{
    public static DateTime ToDateTime(this string input)
    {
        var isDateTime = DateTime.TryParse(input, out var result);
        return isDateTime ? result : DateTime.MinValue;
    }

    public static bool IsMailAddress(this string input)
    {
        return MailAddress.TryCreate(input, out var result);
    }

    public static bool IsNameFormat(this string input)
    {
        const string pattern = @"^[A-Za-z]+(?:\s[A-Za-z0-9]+)*$";

        return Regex.IsMatch(input, pattern);
    }
}