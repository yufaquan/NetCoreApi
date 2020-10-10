using System;
using System.Collections.Generic;
using System.IO;
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
        public static string[] Split(string str, char s)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return new string[0];
            }
            return str.Split(new char[] { s }, StringSplitOptions.RemoveEmptyEntries);
        }

        #region 生成数字和字母的随机字符串
        /// <summary>
        /// 数字和字母随机数
        /// </summary>
        /// <param name="length">生成长度 不包含开头和结尾字符</param>
        /// <param name="startStr">生成长度</param>
        /// <param name="endStr">生成长度</param>
        /// <returns>返回指定长度的数字和字母的随机串</returns>
        public static string RndCode(int length, string startStr, string endStr)
        {
            char[] arrChar = new char[]{
               'a','b','d','c','e','f','g','h','i','j','k','l','m','n','p','r','q','s','t','u','v','w','z','y','x',
               '0','1','2','3','4','5','6','7','8','9',
               'A','B','C','D','E','F','G','H','I','J','K','L','M','N','Q','P','R','T','S','V','U','W','X','Y','Z'};
            StringBuilder num = new System.Text.StringBuilder();
            if (!string.IsNullOrWhiteSpace(startStr))
            {
                num.Append(startStr);
            }
            Random rnd = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < length; i++)
            {
                num.Append(arrChar[rnd.Next(0, arrChar.Length)].ToString());
            }
            if (!string.IsNullOrWhiteSpace(endStr))
            {
                num.Append(endStr);
            }
            return num.ToString();
        }
        /// <summary>
        /// 数字和字母随机数
        /// </summary>
        /// <param name="count">集合大小</param>
        /// <param name="len">生成长度 不包含开头和结尾字符</param>
        /// <param name="startStr">生成长度</param>
        /// <param name="endStr">生成长度</param>
        /// <returns>返回指定长度的数字和字母的随机串集合</returns>
        public static IList<string> RndCodeList(int count, int len, string startStr, string endStr)
        {
            IList<string> list = new List<string>();
            for (int i = 0; i < count; i++) list.Add(RndCode(len, startStr, endStr));
            return list;
        }
        #endregion

        #region file <=> base64
        /// <summary>
        ///  文件转换成Base64字符串
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <returns></returns>
        public static string FileToBase64(Stream fs)
        {
            string strRet = null;

            try
            {
                if (fs == null) return null;
                byte[] bt = new byte[fs.Length];
                fs.Read(bt, 0, bt.Length);
                strRet = Convert.ToBase64String(bt);
                fs.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strRet;
        }

        /// <summary>
        /// Base64字符串转换成文件
        /// </summary>
        /// <param name="strInput">base64字符串</param>
        /// <param name="fileName">保存文件的绝对路径</param>
        /// <returns></returns>
        public static bool Base64ToFileAndSave(string strInput, string fileName)
        {
            bool bTrue = false;

            try
            {
                byte[] buffer = Convert.FromBase64String(strInput);
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();
                bTrue = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return bTrue;
        } 
        #endregion

    }
}
