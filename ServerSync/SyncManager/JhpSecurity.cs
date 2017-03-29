using System.Text;
using System.Security.Cryptography;

namespace JhpDataSystem
{
    public class JhpSecurity
    {
        internal static string Decrypt(string jsonValue)
        {
            return jsonValue;
        }

        internal static string Encrypt(string textToEncrypt)
        {
            return ComputeSha512Hash(textToEncrypt);
        }

        public static string ComputeSha512Hash(string textToEncrypt)
        {
            var sha512 = new SHA512Managed();
            var hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(textToEncrypt));
            var builder = new StringBuilder();

            for (int i = 0; i < hashBytes.Length; i++)
            {
                builder.Append(hashBytes[i].ToString("x2"));
            }            
            return builder.ToString().Substring(6, 30);
        }
    }
}