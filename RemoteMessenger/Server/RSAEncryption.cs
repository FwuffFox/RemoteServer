using System.Security.Cryptography;
using System.Text;

namespace RemoteMessenger.Server;

public static class RSAEncryption
{
    private const string KeyFilePath = "/generated_files/rsa_key.xml";
    public static RSAParameters ServerRSAParameters;
    public static string ServerPublicRSAKeyBase64;
    public static string ServerPrivateRSAKeyBase64;
    public static byte[] ServerPublicRSAKey;
    public static byte[] ServerPrivateRSAKey;

    public static RSAParameters Initialize()
    {
        if (File.Exists(KeyFilePath)) LoadFromKeyFile();
        else GenerateAndSave();
        using var rsa = RSA.Create(2048);
        rsa.ImportParameters(ServerRSAParameters);
        ServerPublicRSAKey = rsa.ExportRSAPublicKey();
        ServerPrivateRSAKey = rsa.ExportRSAPrivateKey();
        ServerPublicRSAKeyBase64 = Convert.ToBase64String(ServerPublicRSAKey);
        ServerPrivateRSAKeyBase64 = Convert.ToBase64String(ServerPrivateRSAKey);
        return ServerRSAParameters;
    }

    private static void LoadFromKeyFile()
    { 
        using var rsa = RSA.Create(2048);
        using var keyFileRs = new StreamReader("generated_files/rsa_key.xml", Encoding.UTF8);
        var xmlString = keyFileRs.ReadToEnd();
        rsa.FromXmlString(xmlString);
        ServerRSAParameters = rsa.ExportParameters(true);
    }

    private static void GenerateAndSave()
    {
        using var keyFileSw = new StreamWriter("generated_files/rsa_key", Encoding.UTF8, new FileStreamOptions
        {
            Mode = FileMode.OpenOrCreate,
            Access = FileAccess.Write
        });
        using var rsa = RSA.Create(2048);
        var xmlString = rsa.ToXmlString(true);
        keyFileSw.Write(xmlString);
        ServerRSAParameters = rsa.ExportParameters(true);
    }

    public static byte[] Decrypt_Bytes(byte[] encryptedBytes)
    {
        using var rsa = RSA.Create(2048);
        rsa.ImportRSAPrivateKey(ServerPrivateRSAKey, out _);
        return rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.Pkcs1);
    }

    public static byte[] Decrypt_Bytes(string encryptedBase64)
    {
        var encryptedBytes = Convert.FromBase64String(encryptedBase64);
        return Decrypt_Bytes(encryptedBytes);
    }

    public static string Decrypt_Base64(string encryptedBase64)
    {
        var decryptedBytes = Decrypt_Bytes(encryptedBase64);
        return Convert.ToBase64String(decryptedBytes);
    }

    public static bool TryDecrypt_Bytes(byte[] encryptedBytes, out byte[] decryptedBytes)
    {
        try
        {
            decryptedBytes = Decrypt_Bytes(encryptedBytes);
            return true;
        }
        catch (Exception e)
        {
            decryptedBytes = Array.Empty<byte>();
            return false;
        }
    }
    
    public static bool TryDecrypt_Bytes(string encryptedBase64, out byte[] decryptedBytes)
    {
        try
        {
            decryptedBytes = Decrypt_Bytes(encryptedBase64);
            return true;
        }
        catch (Exception e)
        {
            decryptedBytes = Array.Empty<byte>();
            return false;
        }
        
    }
}