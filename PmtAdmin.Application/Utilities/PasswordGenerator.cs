using System;
using System.Text;
using PmtAdmin.Application.Services;

namespace PmtAdmin.Application.Utilities
{
    public static class PasswordGenerator
    {
        public static (string password, string hash) GeneratePasswordFromUserInfo(string email, string name)
        {
            // Take first 3 chars of email and last 3 chars of name (if available)
            var emailPart = email.Length >= 3 ? email.Substring(0, 3) : email;
            var namePart = name.Length >= 3 ? name.Substring(name.Length - 3) : name;

            // Add random numbers
            var random = new Random();
            var randomNum = random.Next(1000, 9999).ToString();

            // Combine parts and add special character
            var password = $"{emailPart}{namePart}#{randomNum}";

            // Generate hash using BCrypt (same as in PasswordHashingService)
            var hash = BCrypt.Net.BCrypt.HashPassword(password);

            return (password, hash);
        }
    }
}







//using System;
//using System.Security.Cryptography;
//using System.Text;

//namespace PmtAdmin.Application.Utilities
//{
//    public static class PasswordGenerator
//    {

       
//        public static (string password, string hash) GeneratePasswordFromUserInfo(string email, string name)
//        {
//            // Take first 3 chars of email and last 3 chars of name (if available)
//            var emailPart = email.Length >= 3 ? email.Substring(0, 3) : email;
//            var namePart = name.Length >= 3 ? name.Substring(name.Length - 3) : name;
            
//            // Add random numbers
//            var random = new Random();
//            var randomNum = random.Next(1000, 9999).ToString();
            
//            // Combine parts and add special character
//            var password = $"{emailPart}{namePart}#{randomNum}";
            
//            // Generate hash
//            var hash = HashPassword(password);
            
//            return (password, hash);
//        }

//        public static string HashPassword(string password)
//        {
//            using (var sha256 = SHA256.Create())
//            {
//                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
//                return Convert.ToBase64String(hashedBytes);
//            }
//        }
//    }
//}