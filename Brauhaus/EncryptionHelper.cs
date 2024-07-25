using System.IO;
using System.Security.Cryptography;

namespace Brauhaus;

public static class EncryptionHelper
{
    public static string EncryptString(string plainText, string key)
    {
        using var aesAlg = Aes.Create();
        aesAlg.Key = Convert.FromBase64String(key);
        aesAlg.IV = GenerateRandomIv();

        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }

        var encrypted = msEncrypt.ToArray();
        var ivPlusEncrypted = new byte[aesAlg.IV.Length + encrypted.Length];
        Array.Copy(aesAlg.IV, 0, ivPlusEncrypted, 0, aesAlg.IV.Length);
        Array.Copy(encrypted, 0, ivPlusEncrypted, aesAlg.IV.Length, encrypted.Length);

        return Convert.ToBase64String(ivPlusEncrypted);
    }

    public static string DecryptString(string encryptedText, string key)
    {
        var ivPlusEncrypted = Convert.FromBase64String(encryptedText);
        using var aesAlg = Aes.Create();
        aesAlg.Key = Convert.FromBase64String(key);

        var iv = new byte[aesAlg.IV.Length];
        var encrypted = new byte[ivPlusEncrypted.Length - aesAlg.IV.Length];
        Array.Copy(ivPlusEncrypted, 0, iv, 0, iv.Length);
        Array.Copy(ivPlusEncrypted, iv.Length, encrypted, 0, encrypted.Length);

        aesAlg.IV = iv;
        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
        using var msDecrypt = new MemoryStream(encrypted);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        return srDecrypt.ReadToEnd();
    }

    private static byte[] GenerateRandomIv()
    {
        var iv = new byte[16];
        RandomNumberGenerator.Create().GetBytes(iv);
        return iv;
    }
}