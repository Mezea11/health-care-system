namespace App;

using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;


public static class PasswordHelper
{
    // Generate a salted PBKDF2 hash
    public static (string Hash, string Salt) HashPassword(string password)
    {
        Console.WriteLine("\n--- START: HashPasswordSalted ---");
        Console.WriteLine($"Inkommande lösenord: '{password}'");

        // Generate ett slumpmässigt 
        // byte[] saltBytes = RandomNumberGenerator.GetBytes(16);
        string saltString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
        Console.WriteLine($"1. Salt (16 bytes) genererades slumpmässigt.");
        Console.WriteLine($"   Salt i Base64-strängformat (lagringsformat): {saltString}");

        // Kombinerar lösenord och saltet. Detta blir en enkel sammanfogning
        string passwordAndSalt = password + saltString;
        byte[] passwordAndSaltBytes = Encoding.UTF8.GetBytes(passwordAndSalt);
        Console.WriteLine("2. Lösenord och Salt slogs ihop till en enda sträng.");
        Console.WriteLine($"   Kombinerad sträng: '{passwordAndSalt}'");
        Console.WriteLine($"   Strängen konverterades till bytes ({passwordAndSaltBytes.Length} bytes) för hashing (UTF8).");


        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] hashBytes = sha256Hash.ComputeHash(passwordAndSaltBytes);
            string hashString = Convert.ToBase64String(hashBytes);
            Console.WriteLine("3. SHA256 hashing utfördes EN GÅNG på de kombinerade bytesen.");
            Console.WriteLine($"   Resultatet är 32 bytes (256 bitar).");
            Console.WriteLine($"   Hash i Base64-strängformat (lagringsformat): {hashString}");

            Console.WriteLine("--- SLUT: HashPasswordSalted ---\n");
            return (hashString, saltString);
        }
        // Derive a 256-bit subkey (32 bytes)
        // using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256);
        // byte[] hashBytes = pbkdf2.GetBytes(32);

        // return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
    }

    // verify ppassword against stored hash + salt
    public static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        Console.WriteLine("\n--- START: VerifyPasswordSalted ---");
        Console.WriteLine($"Inkommande lösenord att verifiera: '{password}'");
        Console.WriteLine($"Lagrad Hash: {storedHash}");
        Console.WriteLine($"Lagrat Salt: {storedSalt}");

        // Börja med att återskapa den komberade strängen 
        string passwordAndSalt = password + storedSalt;
        byte[] passwordandSaltBytes = Encoding.UTF8.GetBytes(passwordAndSalt);
        Console.WriteLine("1. Det nya lösenordet och det lagrade Saltet slogs ihop igen.");
        Console.WriteLine($"   Kombinerad sträng: '{passwordAndSalt}'");
        Console.WriteLine($"   Strängen konverterades till bytes ({passwordandSaltBytes.Length} bytes).");

        // Hasha 
        using (SHA256 sha26Hash = SHA256.Create())
        {
            byte[] hashBytes = sha26Hash.ComputeHash(passwordandSaltBytes);
            string computedHash = Convert.ToBase64String(hashBytes);
            Console.WriteLine("2. Hashning (SHA256) utfördes på de nya kombinerade bytesen.");
            Console.WriteLine($"  NYBERÄKNAD Hash: {computedHash}");

            Console.WriteLine($"3. Jämförelse: Nyberäknad Hash == Lagrad Hash? -> {computedHash == storedHash}");
            Console.WriteLine("--- SLUT: VerifyPasswordSalted ---\n");

            return computedHash == storedHash;

        }

        // using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256);
        // byte[] hashBytes = pbkdf2.GetBytes(32);

        // string computedHash = Convert.ToBase64String(hashBytes);
        // return computedHash == storedHash;
    }

}