using System.IO;
using System.Text;
using System.Security.Cryptography;
using System;

public static class EncryptionHelper
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