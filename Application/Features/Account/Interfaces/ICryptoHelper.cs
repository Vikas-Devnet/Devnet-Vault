namespace Application.Features.Account.Interfaces;

public interface ICryptoHelper
{
    byte[] GenerateRandomBytes(int length);
    byte[] DeriveKey(string password, byte[] salt);
    (byte[] cipher, byte[] iv, byte[] tag) Encrypt(byte[] data, byte[] key);
    byte[] Decrypt(byte[] cipher, byte[] key, byte[] iv, byte[] tag);
}
