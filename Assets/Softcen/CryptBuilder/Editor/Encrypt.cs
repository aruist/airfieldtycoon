using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

public class Encrypt
{
    public static string EncryptString(string plainText)
    {
        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] keyBytes = new Rfc2898DeriveBytes(GameConsts.pGq2Vlmr, Encoding.ASCII.GetBytes(GameConsts.sZvWe9sJ)).GetBytes(256 / 8);

        var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
        var encrypter = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(GameConsts.vs43xKOY));

        byte[] cipherTextBytes;

        using (var memoryStream = new MemoryStream())
        {
            using (var cryptoStream = new CryptoStream(memoryStream, encrypter, CryptoStreamMode.Write))
            {
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                cipherTextBytes = memoryStream.ToArray();
                cryptoStream.Close();
            }
            memoryStream.Close();
        }
        return Convert.ToBase64String(cipherTextBytes);
    }
}
