using BattleToad.XMLHelper;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System;

namespace BattleToad.Crypt
{
    public static class AesHelper
    {
        /// <summary>
        /// Cоль
        /// </summary>
        public static byte[] Salt =
                    new byte[] {
                    0x23,
                    0x87,
                    0x44,
                    0x1f,
                    0x1c,
                    0x3a,
                    0x77,
                    0x17,
                    0x9F,
                    0x64,
                    0x66,
                    0x11,
                    0xaf
                    };
        /// <summary>
        /// Зашифровать строку
        /// </summary>
        /// <param name="text">Строка</param>
        /// <param name="password">Пароль для шифрования</param>
        /// <returns>Зашифрованная строка</returns>
        public static string Encrypt(this string text, string password) => Encrypting(text, password);
        /// <summary>
        /// раcшифровать строку
        /// </summary>
        /// <param name="text">Строка</param>
        /// <param name="password">Пароль для шифрования</param>
        /// <returns>Строка</returns>
        public static string Decrypt(this string text, string password) => Decrypting(text, password);
        /// <summary>
        /// Зашифровать строку
        /// </summary>
        /// <param name="text">Строка</param>
        /// <param name="password">Пароль для шифрования</param>
        /// <returns>Зашифрованная строка</returns>
        public static string Encrypting(string text, string password)
            => Convert.ToBase64String(Crypta(Encoding.Unicode.GetBytes(text), password, false));
        /// <summary>
        /// раcшифровать строку
        /// </summary>
        /// <param name="text">Строка</param>
        /// <param name="password">Пароль для шифрования</param>
        /// <returns>Строка</returns>
        public static string Decrypting(string text, string password)
            => Encoding.Unicode.GetString(Crypta(Convert.FromBase64String(text), password, true));

        /// <summary>
        /// Зашифровать или раcшифровать массив байт
        /// </summary>
        /// <param name="cipherBytes">байты</param>
        /// <param name="password">пароль</param>
        /// <param name="decrypt">режим расшифровки, False - зашифровать, True - раcшифровать</param>
        /// <returns>Массив байт</returns>
        public static byte[] Crypta(byte[] cipherBytes, string password, bool decrypt)
        {
            byte[] result;
            using (Aes encryptor = Aes.Create())
            {
                using (Rfc2898DeriveBytes derivebytes = new Rfc2898DeriveBytes(password, Salt))
                {
                    encryptor.Key = derivebytes.GetBytes(32);
                    encryptor.IV = derivebytes.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (
                            CryptoStream cs =
                            decrypt ?
                            new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write)
                            :
                            new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write)
                            )
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        result = ms.ToArray();
                    }
                }
            }
            return result;
        }
    }
    /// <summary>
    /// Класс-помощник для работы с шифрованием RSA
    /// </summary>
    public class RSAHelper : IDisposable
    {
        /// <summary>
        /// Пресеты размера ключа
        /// </summary>
        public static class RSA_KEY_LENGTH
        {
            public const int RSA128 = 128;
            public const int RSA256 = 256;
            public const int RSA512 = 512;
            public const int RSA1024 = 1024;
            public const int RSA2048 = 2048;
            public const int RSA4096 = 4096;
            public const int RSA8192 = 8192;
        }
        private readonly RSACryptoServiceProvider RSA;
        private RSAParameters Key_Private;
        private RSAParameters Key_Public;
        /// <summary>
        /// Приватный ключ
        /// </summary>
        public string PrivateKey
        {
            set => Key_Private = value.FromXML<RSAParameters>();
            get => Key_Private.ToXML();
        }
        /// <summary>
        /// Публичный ключ
        /// </summary>
        public string PublicKey
        {
            set => Key_Public = value.FromXML<RSAParameters>();
            get => Key_Public.ToXML();
        }
        /// <summary>
        /// Создать экземпляр класса RSAHelper
        /// </summary>
        /// <param name="KeySize">размер ключа</param>
        public RSAHelper(int KeySize = 512)
        {
            RSA = new RSACryptoServiceProvider(KeySize);
            Key_Private = RSA.ExportParameters(true);
            Key_Public = RSA.ExportParameters(false);
        }
        /// <summary>
        /// Создать экземпляр класса RSAHelper, сразу указав приватный
        /// </summary>
        /// <param name="keySize">размер ключа</param>
        /// <param name="privateKey">приватный ключ</param>
        public RSAHelper(int keySize, string privateKey)
        {
            RSA = new RSACryptoServiceProvider(keySize);
            PrivateKey = privateKey;
            Key_Public = RSA.ExportParameters(false);
        }
        private bool disposed = false;
        /// <summary>
        /// Убить экземпляр класса, а что он тебе сделал?
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    RSA.Dispose();
                }
                disposed = true;
            }
        }
        ~RSAHelper()
        {
            Dispose(false);
        }

        /// <summary>
        /// Зашифровать строку
        /// </summary>
        /// <param name="DataToEncrypt">строка</param>
        /// <returns>зашифрованная строка</returns>
        public string Encrypt(string DataToEncrypt) =>
            Convert.ToBase64String(Encrypt(Encoding.Unicode.GetBytes(DataToEncrypt), Key_Public));
        /// <summary>
        /// Расшифровать строку
        /// </summary>
        /// <param name="DataToEncrypt">шифрованная строка</param>
        /// <returns>расшифрованная строка</returns>
        public string Decrypt(string DataToEncrypt) =>
        Encoding.Unicode.GetString(Decrypt(Convert.FromBase64String(DataToEncrypt), Key_Private));

        public byte[] Encrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo,
                                  bool DoOAEPPadding = false)
        {
            RSA.ImportParameters(RSAKeyInfo);
            return RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
        }

        public byte[] Decrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo,
                                  bool DoOAEPPadding = false)
        {
            RSA.ImportParameters(RSAKeyInfo);
            return RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
        }
        /// <summary>
        /// Зашифровать строку асинхронно
        /// </summary>
        /// <param name="DataToEncrypt">строка</param>
        /// <returns>зашифрованная строка</returns>
        public async Task<string> EncryptAsync(string DataToEncrypt) =>
            Convert.ToBase64String(
                await EncryptAsync(Encoding.Unicode.GetBytes(DataToEncrypt), Key_Public)
                );
        /// <summary>
        /// Расшифровать строку асинхронно
        /// </summary>
        /// <param name="DataToEncrypt">шифрованная строка</param>
        /// <returns>расшифрованная строка</returns>
        public async Task<string> DecryptAsync(string DataToEncrypt) =>
            Encoding.Unicode.GetString(
                await DecryptAsync(Convert.FromBase64String(DataToEncrypt), Key_Private)
                );

        public async Task<byte[]> EncryptAsync(byte[] DataToEncrypt, RSAParameters RSAKeyInfo,
                                               bool DoOAEPPadding = false)
            => await Task.Run(() => Encrypt(DataToEncrypt, RSAKeyInfo, DoOAEPPadding));

        public async Task<byte[]> DecryptAsync(byte[] DataToEncrypt, RSAParameters RSAKeyInfo,
                                               bool DoOAEPPadding = false)
            => await Task.Run(() => Decrypt(DataToEncrypt, RSAKeyInfo, DoOAEPPadding));
    }
}