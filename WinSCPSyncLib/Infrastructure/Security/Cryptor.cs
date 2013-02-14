using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WinSCPSyncLib.Infrastructure.Security
{
    public class Cryptor
    {
        private static byte[] _key;
        private static byte[] _iv;
        private static TripleDESCryptoServiceProvider _provider;

        static Cryptor()
        {
            // change in app.config for both ui client and windows service
            _key = System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["CryptorSecretKey"]);
            _iv = System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["CryptorInitializationVector"]);
            _provider = new TripleDESCryptoServiceProvider();
        }

        public static string Encrypt(string plainText)
        {
            return Transform(plainText, _provider.CreateEncryptor(_key, _iv));
        }

        public static string Decrypt(string encryptedText)
        {
            return Transform(encryptedText, _provider.CreateDecryptor(_key, _iv));
        }

        private static string Transform(string text, ICryptoTransform transform)
        {
            if (text == null)
            {
                return null;
            }

            using (MemoryStream stream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Write))
                {
                    byte[] input = Encoding.Default.GetBytes(text);
                    cryptoStream.Write(input, 0, input.Length);
                    cryptoStream.FlushFinalBlock();

                    return Encoding.Default.GetString(stream.ToArray());
                }
            }
        }
    }
}
