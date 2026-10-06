using System.Security.Cryptography;

namespace SchoolManagement.Infrastructure.Security;

/// <summary>Generates readable temporary passwords for new accounts.</summary>
public static class PasswordGenerator
{
    // Avoids ambiguous characters (0/O, 1/l/I).
    private const string Chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";

    public static string Generate(int length = 10)
    {
        var sb = new System.Text.StringBuilder(length);
        var bytes = RandomNumberGenerator.GetBytes(length);
        for (var i = 0; i < length; i++)
            sb.Append(Chars[bytes[i] % Chars.Length]);
        // Ensure it has a digit and an uppercase for complexity rules.
        sb[0] = char.ToUpperInvariant(sb[0] is >= 'a' and <= 'z' ? sb[0] : 'K');
        sb[length - 1] = (char)('2' + (bytes[0] % 8));
        return sb.ToString();
    }
}
