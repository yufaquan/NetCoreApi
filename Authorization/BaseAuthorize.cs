using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authorization
{
    public abstract class BaseAuthorize<T> where T : class
    {
        public abstract string Text { get; }

        public string GetText()
        {
            return Text + typeof(T).GetDisplayName();
        }

    }

    [YDisplay("创建")]
    public class Create<T> : BaseAuthorize<T> where T : class
    {
        public override string Text => "创建";


    }
    [YDisplay("修改")]
    public class Update<T> : BaseAuthorize<T> where T : class
    {
        public override string Text => "修改";
    }
    [YDisplay("查看")]
    public class Read<T> : BaseAuthorize<T> where T : class
    {
        public override string Text => "查看";
    }
    [YDisplay("删除")]
    public class Delete<T> : BaseAuthorize<T> where T : class
    {
        public override string Text => "删除";
    }
    [YDisplay("权限")]
    public class Authorize<T> : BaseAuthorize<T> where T : class
    {
        public override string Text => "权限";
    }


    public class MyAuthorizeKeyValue
    {
        /// <summary>
        /// 权限
        /// </summary>
        public Type Key { get; set; }
        /// <summary>
        /// model
        /// </summary>
        public Type Value { get; set; }

        public string GetString()
        {
            return Key.ToString();
        }


        public string GetText()
        {
            return Key.GetDisplayName() + Value.GetDisplayName();
        }
    }

    /// <summary>
    /// 全部权限数据
    /// </summary>
    public static class AuthorizeData
    {
        private static List<MyAuthorizeKeyValue> authorizeList = new List<MyAuthorizeKeyValue>();

        public static void Add(Type key, Type value)
        {
            authorizeList.Add(new MyAuthorizeKeyValue() { Key = key, Value = value });
        }


        public static List<string> GetAuthorizeStringList()
        {
            List<string> result = new List<string>();
            if (authorizeList != null)
            {
                foreach (var item in authorizeList)
                {
                    result.Add(item.GetString());
                }
            }
            return result;
        }

        /// <summary>
        /// 读取权限名称
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, string>> GetTextAndString()
        {
            var result = new Dictionary<string, Dictionary<string, string>>();
            foreach (var item in authorizeList)
            {
                var className = item.Value.GetDisplayName();//类名
                var aName = item.Key.GetDisplayName();//权限名称
                if (result.ContainsKey(className))
                {
                    if (!result[className].ContainsKey(aName))
                    {
                        result[className].Add(aName, item.GetString());
                    }
                }
                else
                {
                    var cdata = new Dictionary<string, string>();
                    cdata.Add(aName, item.GetString());
                    result.Add(className, cdata);
                }
            }
            return result;
        }


    }
}
