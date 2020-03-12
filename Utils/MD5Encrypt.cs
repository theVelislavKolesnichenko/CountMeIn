using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Utils
{
    public static class MD5Encrypt
    {
        public static string MD5Hash(string intput)
        {
            if (intput == null)
                return string.Empty;

            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(intput));
            byte[] dataHash = md5Hash.ComputeHash(data);

            StringBuilder strBuffer = new StringBuilder();

            for (int i = 0; i < dataHash.Length; i++)
            {
                strBuffer.Append(dataHash[i].ToString("x2"));
            }

            return strBuffer.ToString();
        }
    }

}
