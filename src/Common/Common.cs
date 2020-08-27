using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    public class Commons
    {
        /// <summary>
        /// MD5不可逆加密   32位加密
        /// </summary>
        /// <param name="s"></param>
        /// <param name="_input_charset"></param>
        /// <returns></returns>
        public static string GetMD5_32(string s)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding("utf-8").GetBytes(s));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString().ToUpper();
        }

        /// <summary>
        /// 首个字符大写 为空返回原字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            return input.First().ToString().ToUpper() + input.Substring(1);
        }


        /// <summary>
        /// 将字符串按特定字符切割成字符串数组
        /// </summary>
        /// <param name="str">需要切割的字符串</param>
        /// <param name="s">指定字符</param>
        /// <returns></returns>
        public static string[] Split(string str,char s)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return new string[0];
            }
            return str.Split(new char[] { s }, StringSplitOptions.RemoveEmptyEntries);
        }

    }
}
