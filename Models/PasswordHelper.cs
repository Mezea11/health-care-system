namespace App;

using System.Security.Cryptography;

public static class PasswordHelper
{
    // Generate a salted PBKDF2 hash
    public static (string Hash, string Salt) HashPassword(string password)
    {
        // Generate a random salt
        byte[] saltBytes = RandomNumberGenerator.GetBytes(16);

        // Derive a 256-bit subkey (32 bytes)
        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256);
        byte[] hashBytes = pbkdf2.GetBytes(32);

        return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
    }

    // verify ppassword against stored hash + salt
    public static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        byte[] saltBytes = Convert.FromBase64String(storedSalt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256);
        byte[] hashBytes = pbkdf2.GetBytes(32);

        string computedHash = Convert.ToBase64String(hashBytes);
        return computedHash == storedHash;
    }

}