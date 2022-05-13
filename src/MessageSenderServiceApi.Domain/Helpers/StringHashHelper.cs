using System.Security.Cryptography;
using System.Text;

namespace MessageSenderServiceApi.Domain.Helpers;

public interface IStringHashHelper
{
    string GetStringHashSHA512(string data);
}

public class StringHashHelper : IStringHashHelper
{
    public string GetStringHashSHA512(string data)
    {
        using var sha1 = SHA512.Create();
        var hashSh1 = sha1.ComputeHash(Encoding.UTF8.GetBytes(data));

        var sb = new StringBuilder(hashSh1.Length * 2);

        foreach (byte b in hashSh1)
        {
            sb.Append(b.ToString("X2").ToLower());
        }

        return sb.ToString();
    }
}