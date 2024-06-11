namespace Arshdny_ProjectV2.Utilites
{
    public class Utilites
    {
        // Function to hash (encrypt) the password
        public static string HashPassword(string password)
        {
            // Generate a salt and hash the password
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
        }

        // Function to verify (decrypt) the password
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Verify the password against the hash
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
