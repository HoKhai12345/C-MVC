using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NET_MVC.Helpers
{
    public static class HashCodeHelper
    {
        public static string GetMd5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Chuyển thành chuỗi Hex
                return Convert.ToHexString(hashBytes).ToLower();
            }
        }
        public static string ToMd5(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            using (MD5 md5 = MD5.Create())
            {
                // Chuyển chuỗi thành mảng byte
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                // Mã hóa
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Chuyển mảng byte thành chuỗi Hex (giống kết quả md5() của PHP)
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}