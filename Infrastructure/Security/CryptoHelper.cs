using Application.Features.Account.Interfaces;
using System.Security.Cryptography;

namespace Infrastructure.Security;

public class CryptoHelper : ICryptoHelper
{
    private const int KeySize = 32;
    private const int IvSize = 12;
    private const int TagSize = 16;
    private const int Iterations = 100000;

    public byte[] GenerateRandomBytes(int length)
    {
        var bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes);
        return bytes;
    }

    public byte[] DeriveKey(string password, byte[] salt)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty");

        if (salt == null || salt.Length < 8)
            throw new ArgumentException("Invalid salt");

        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256);

        return pbkdf2.GetBytes(KeySize);
    }

    public (byte[] cipher, byte[] iv, byte[] tag) Encrypt(byte[] data, byte[] key)
    {
        if (data == null || data.Length == 0)
            throw new ArgumentException("Data cannot be empty");

        if (key == null || key.Length != KeySize)
            throw new ArgumentException("Invalid key size");

        var iv = GenerateRandomBytes(IvSize);
        var cipher = new byte[data.Length];
        var tag = new byte[TagSize];

        using var aes = new AesGcm(key, TagSize);

        aes.Encrypt(iv, data, cipher, tag);

        return (cipher, iv, tag);
    }

    public byte[] Decrypt(byte[] cipher, byte[] key, byte[] iv, byte[] tag)
    {
        if (cipher == null || cipher.Length == 0)
            throw new ArgumentException("Cipher cannot be empty");

        if (key == null || key.Length != KeySize)
            throw new ArgumentException("Invalid key size");

        if (iv == null || iv.Length != IvSize)
            throw new ArgumentException("Invalid IV");

        if (tag == null || tag.Length != TagSize)
            throw new ArgumentException("Invalid tag");

        var plaintext = new byte[cipher.Length];

        using var aes = new AesGcm(key, TagSize);

        aes.Decrypt(iv, cipher, tag, plaintext);

        return plaintext;
    }
}