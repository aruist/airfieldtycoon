using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

// Decrypt
public class M4hVva1c {
    // DecryptString
    public static string VEV91MPb(string i8wDTXBL)
    {
        byte[] mZoQWYTu = Convert.FromBase64String(i8wDTXBL);
        byte[] PFogFgiQ = new Rfc2898DeriveBytes(GameConsts.pGq2Vlmr, Encoding.ASCII.GetBytes(GameConsts.sZvWe9sJ)).GetBytes(256 / 8);
        var kTdCKDJP = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };
        var MGkMJWGP = kTdCKDJP.CreateDecryptor(PFogFgiQ, Encoding.ASCII.GetBytes(GameConsts.vs43xKOY));
        var MEAtXLcv = new MemoryStream(mZoQWYTu);
        var nFqeQtUt = new CryptoStream(MEAtXLcv, MGkMJWGP, CryptoStreamMode.Read);
        byte[] WKGxECUU = new byte[mZoQWYTu.Length];

        int wCzVeIXL = nFqeQtUt.Read(WKGxECUU, 0, WKGxECUU.Length);
        MEAtXLcv.Close();
        nFqeQtUt.Close();

        return Encoding.UTF8.GetString(WKGxECUU, 0, wCzVeIXL).TrimEnd("\0".ToCharArray());
    }
    // GetStr
    public static string ZTGjqBkg(int[] WTevzbYD)
    {
        string TWqkiTvI = "";
        for (int i = 0; i < WTevzbYD.Length; i++)
        {
            TWqkiTvI += (char)WTevzbYD[i];
        }
        return VEV91MPb(TWqkiTvI);
    }

}
