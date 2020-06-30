using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MultiplePasswordsEncrypter
{
    // Crypto module
    public class Crypto
    {
        public static readonly Aes _aes = new AesCryptoServiceProvider();

        public const int AES_KEY_SIZE = 256;    // bit
        public const int AES_BLOCK_SIZE = 128;  // bit

        public const int AES_KEY_SIZE_BYTE = AES_KEY_SIZE / 8;      // byte
        public const int AES_BLOCK_SIZE_BITE = AES_BLOCK_SIZE / 8;  // byte

        public const int AES_KEY_SIZE_BYTE_WITH_PADDING = AES_KEY_SIZE_BYTE + 16;   // 16 for padding

        public static readonly byte[] DEFAULT_SALT = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
        public const int DEFAULT_ITERATIONS = 1000;
        
        public static byte[] encryptByte(byte[] data, byte[] password, byte[] salt = null, int iterations = DEFAULT_ITERATIONS)
        {
            if (salt == null)
            {
                salt = DEFAULT_SALT;
            }

            System.Security.Cryptography.Rfc2898DeriveBytes deriveBytes = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt, iterations);
            byte[] key = deriveBytes.GetBytes(AES_KEY_SIZE_BYTE);
            byte[] iv = deriveBytes.GetBytes(AES_BLOCK_SIZE_BITE);
            using (ICryptoTransform encryptor = _aes.CreateEncryptor(key, iv))
            {
                return encryptor.TransformFinalBlock(data, 0, data.Length);
            }
        }

        public static void encryptStream(Stream inStream, Stream outStream, byte[] password, byte[] salt = null, int iterations = DEFAULT_ITERATIONS)
        {
            if (salt == null)
            {
                salt = DEFAULT_SALT;
            }

            System.Security.Cryptography.Rfc2898DeriveBytes deriveBytes = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt, iterations);
            byte[] key = deriveBytes.GetBytes(AES_KEY_SIZE_BYTE);
            byte[] iv = deriveBytes.GetBytes(AES_BLOCK_SIZE_BITE);
            using (ICryptoTransform encryptor = _aes.CreateEncryptor(key, iv))
            {
                using (var cs = new System.Security.Cryptography.CryptoStream(outStream, encryptor, System.Security.Cryptography.CryptoStreamMode.Write))
                {
                    inStream.CopyTo(cs);
                    cs.FlushFinalBlock();
                }
            }
        }


        public static byte[] decryptByte(byte[] data, byte[] password, byte[] salt = null, int iterations = DEFAULT_ITERATIONS)
        {
            return decryptByte(data, 0, data.Length, password, salt, iterations);
        }

        public static byte[] decryptByte(byte[] data, int from, int len, byte[] password, byte[] salt = null, int iterations = DEFAULT_ITERATIONS)
        {
            if (salt == null)
            {
                salt = DEFAULT_SALT;
            }

            System.Security.Cryptography.Rfc2898DeriveBytes deriveBytes = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt, iterations);
            byte[] key = deriveBytes.GetBytes(AES_KEY_SIZE_BYTE);
            byte[] iv = deriveBytes.GetBytes(AES_BLOCK_SIZE_BITE);
            using (ICryptoTransform decryptor = _aes.CreateDecryptor(key, iv))
            {
                try
                {
                    return decryptor.TransformFinalBlock(data, from, len);
                }
                catch (CryptographicException)
                {
                    return null;
                }
            }
        }

        public static void decryptStream(Stream inStream, Stream outStream, byte[] password, byte[] salt = null, int iterations = DEFAULT_ITERATIONS)
        {
            if (salt == null)
            {
                salt = DEFAULT_SALT;
            }

            System.Security.Cryptography.Rfc2898DeriveBytes deriveBytes = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt, iterations);
            byte[] key = deriveBytes.GetBytes(Crypto.AES_KEY_SIZE / 8);
            byte[] iv = deriveBytes.GetBytes(Crypto.AES_BLOCK_SIZE / 8);
            using (System.Security.Cryptography.ICryptoTransform decryptor = Crypto._aes.CreateDecryptor(key, iv))
            {
                using (var cs = new System.Security.Cryptography.CryptoStream(outStream, decryptor, System.Security.Cryptography.CryptoStreamMode.Write))
                {
                    inStream.CopyTo(cs);
                    cs.FlushFinalBlock();
                }
            }
        }


        public static byte[] createSalt(int a, int b)
        {
            return BitConverter.GetBytes((long)a << 32 | (long)b);
        }
    }
}
