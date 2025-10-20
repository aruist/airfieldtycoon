using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

public class Decrypt
{
    //static readonly string PasswordHash = "Passworddiipadaipa";
    // SaltKey must be 16 length
    //static readonly string SaltKey = "Saltkey12345678";
    // VIKey must be 16 length
    //static readonly string VIKey = "1234567789123456";

    public static string DecryptString(string encryptedText)
    {
        byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
        byte[] keyBytes = new Rfc2898DeriveBytes(GameConsts.pGq2Vlmr, Encoding.ASCII.GetBytes(GameConsts.sZvWe9sJ)).GetBytes(256 / 8);
        var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };
        var decypter = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(GameConsts.vs43xKOY));
        var memoryStream = new MemoryStream(cipherTextBytes);
        var cryptoStream = new CryptoStream(memoryStream, decypter, CryptoStreamMode.Read);
        byte[] plainTextBytes = new byte[cipherTextBytes.Length];

        int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
        memoryStream.Close();
        cryptoStream.Close();

        return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
    }

    public static string GetStr(int[] buffer)
    {
        string str = "";
        for (int i=0; i < buffer.Length; i++)
        {
            str += (char)buffer[i];
        }
        return DecryptString(str);
    }
}
