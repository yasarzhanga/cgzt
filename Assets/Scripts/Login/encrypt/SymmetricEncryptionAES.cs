using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class SymmetricEncryptionAES : MonoBehaviour
{
    /// <summary>
    /// AES 对称加密工具类
    /// 提供 AES-256 加密和解密功能，用于安全传输敏感数据
    /// 支持随机密钥和初始化向量生成
    /// </summary>
    // 生成随机密钥
    private static byte[] GenerateKey()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] key = new byte[32]; // AES-256需要256位密钥
            rng.GetBytes(key);
            return key;
        }
    }

    // 生成随机初始化向量（IV）
    private static byte[] GenerateIV()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] iv = new byte[16]; // AES使用128位IV
            rng.GetBytes(iv);
            return iv;
        }
    }

    // 使用AES算法加密数据
    public static byte[] EncryptData(string plaintext, out byte[] key, out byte[] iv)
    {
        key = GenerateKey();
        iv = GenerateIV();

        using (var aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (var ms = new MemoryStream())
            {
                ms.Write(iv, 0, iv.Length); // 先写入IV
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plaintext);
                    }
                }
                return ms.ToArray();
            }
        }
    }

    // 使用AES算法解密数据
    public static string DecryptData(byte[] ciphertext, byte[] key, byte[] iv)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (var ms = new MemoryStream(ciphertext, iv.Length, ciphertext.Length - iv.Length))
            {
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (var sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }
    // 保存加密数据
    public void SaveEncryptedData(string plaintext)
    {
        Debug.Log("Original Text: " + plaintext);
        byte[] key, iv;
        byte[] encryptedData = EncryptData(plaintext, out key, out iv);
        Debug.Log("Encrypted Data: " + BitConverter.ToString(encryptedData));
        string base64String = Convert.ToBase64String(encryptedData);
        string base64Stringkey = Convert.ToBase64String(key);
        string base64Stringiv = Convert.ToBase64String(iv);
        PlayerPrefs.SetString("password", base64String);
        PlayerPrefs.SetString("key", base64Stringkey);
        PlayerPrefs.SetString("iv", base64Stringiv);
    }

    // 加载加密数据
    public byte[] LoadEncryptedData(string plaintext)
    {
        string base64String = PlayerPrefs.GetString(plaintext, string.Empty);
        if (string.IsNullOrEmpty(base64String))
        {
            return null;
        }
        // 清理输入数据
        base64String = base64String.Trim().Replace(" ", "");

        // 检查字符串长度是否为4的倍数
        if (base64String.Length % 4 != 0)
        {
            base64String += new String('=', 4 - base64String.Length % 4);
        }

        try
        {
            byte[] data = Convert.FromBase64String(base64String);
            return data;
        }
        catch (FormatException e)
        {
            Console.WriteLine("Invalid Base-64 string: " + e.Message);
            return null;
        }


    }
}
