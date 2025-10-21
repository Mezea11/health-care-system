namespace App;

using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;


public static class PasswordHelper
{
    // Generate a salted hash for the password using a single SHA256 iteration.
    // NOTE: For production, a key derivation function like PBKDF2 (commented out below) 
    // is strongly recommended for added security against brute-force attacks.
    public static (string Hash, string Salt) HashPassword(string password)
    {

        // Generate a random 16-byte salt and convert it to a Base64 string for easy storage.
        string saltString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));

        // --- Salt Application ---
        // Combine the plain-text password with the newly generated salt.
        // This ensures that even two identical passwords will result in different hashes.
        string passwordAndSalt = password + saltString;

        // Convert the combined string into a byte array (using UTF8 encoding)
        // as the hashing algorithm (SHA256) operates on binary data.
        byte[] passwordAndSaltBytes = Encoding.UTF8.GetBytes(passwordAndSalt);


        // --- Hashing Process ---
        // Use the SHA256 algorithm to compute a one-way hash of the salted password bytes.
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] hashBytes = sha256Hash.ComputeHash(passwordAndSaltBytes);

            // Convert the resulting hash bytes into a Base64 string for secure storage in the database/file.
            string hashString = Convert.ToBase64String(hashBytes);

            // Return both the Hash and the Salt. Both must be stored to verify the password later.
            return (hashString, saltString);
        }

        // The following code demonstrates the more secure PBKDF2 method (Rfc2898DeriveBytes), 
        // which is generally preferred as it performs many iterations (e.g., 100,000) 
        // to slow down brute-force attacks.
    }

    // verify password against stored hash + salt
    public static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {

        // --- Recreate the Input ---
        // Combine the *incoming* password with the *stored* salt.
        // This must exactly match the process used in HashPassword to generate the original hash.
        string passwordAndSalt = password + storedSalt;
        byte[] passwordandSaltBytes = Encoding.UTF8.GetBytes(passwordAndSalt);


        // --- Re-hashing and Comparison ---
        // Re-hash the newly combined string to generate the computed hash.
        using (SHA256 sha26Hash = SHA256.Create())
        {
            byte[] hashBytes = sha26Hash.ComputeHash(passwordandSaltBytes);
            string computedHash = Convert.ToBase64String(hashBytes);

            // Compare the newly computed hash against the hash retrieved from storage.
            // If they are identical, the password is correct (True). If not, it's incorrect (False).
            return computedHash == storedHash;

        }
    }

}