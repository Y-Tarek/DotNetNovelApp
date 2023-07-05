using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Novels.Handlers;
public class Hash
{
    public static byte[] GetHash(string password, string salt)
    {  
        if(password == null)
        {
            return null;
        }
        byte[] unhashedBytes = Encoding.Unicode.GetBytes(String.Concat(salt, password));

        SHA256Managed sha256 = new SHA256Managed();
        byte[] hashedBytes = sha256.ComputeHash(unhashedBytes);

        return hashedBytes;
    }

    //public static bool CompareHash(string password, byte[] hash,string salt)
    //{
    //    string decoded = Convert.ToBase64String(hash);
    //    string encodedPassword = Convert.ToBase64String(GetHash(password, salt));
    //    return decoded == encodedPassword;
    //}
}