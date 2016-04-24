using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AutoMapper.EF6.Extended.Tests.Shared.Encryption
{
  /// <summary>
  ///   Don't use this in production, please don't
  /// </summary>
  public static class EncryptionHelper
  {
    private static string _key = "";

    private static string EncryptionKey
    {
      get
      {
        if (string.IsNullOrEmpty(_key))
        {
          _key = @"5f58G{2tTcOAtm?mH23@V2s=E80""0r";
        }

        return _key;
      }
    }

    public static string Encrypt(string clearText)
    {
      var clearBytes = Encoding.Unicode.GetBytes(clearText);
      using (var encryptor = Aes.Create())
      {
        var iv = new byte[15];
        var rand = RandomNumberGenerator.Create();
        rand.GetBytes(iv);
        var pdb = new Rfc2898DeriveBytes(EncryptionKey, iv);
        encryptor.Key = pdb.GetBytes(32);
        encryptor.IV = pdb.GetBytes(16);
        using (var ms = new MemoryStream())
        {
          using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
          {
            cs.Write(clearBytes, 0, clearBytes.Length);
            cs.Close();
          }
          clearText = Convert.ToBase64String(iv) + Convert.ToBase64String(ms.ToArray());
        }
      }
      return clearText;
    }

    public static string Decrypt(string cipherText)
    {
      var iv = Convert.FromBase64String(cipherText.Substring(0, 20));
      cipherText = cipherText.Substring(20).Replace(" ", "+");
      var cipherBytes = Convert.FromBase64String(cipherText);
      using (var encryptor = Aes.Create())
      {
        var pdb = new Rfc2898DeriveBytes(EncryptionKey, iv);
        encryptor.Key = pdb.GetBytes(32);
        encryptor.IV = pdb.GetBytes(16);
        using (var ms = new MemoryStream())
        {
          using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
          {
            cs.Write(cipherBytes, 0, cipherBytes.Length);
            cs.Close();
          }
          cipherText = Encoding.Unicode.GetString(ms.ToArray());
        }
      }
      return cipherText;
    }
  }
}