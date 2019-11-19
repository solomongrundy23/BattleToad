using System.IO;
using System.Text;
using System.Security.Cryptography;
using System;

public static class EncryptionHelper
{
    public static string Encrypt(this string text, string password) => Encrypting(text, password);
    public static string Decrypt(this string text, string password) => Decrypting(text, password);

    private static string Encrypting(string text, string password)
        => Convert.ToBase64String(Crypta(Encoding.Unicode.GetBytes(text), password, false));
    private static string Decrypting(string text, string password)
        => Encoding.Unicode.GetString(Crypta(Convert.FromBase64String(text), password, true));

    private static byte[] Crypta(byte[] cipherBytes, string password, bool decrypt)
    {
        byte[] result;
        using (Aes encryptor = Aes.Create())
        {
            using (Rfc2898DeriveBytes derivebytes = new Rfc2898DeriveBytes(password,
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
                }
                ))
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