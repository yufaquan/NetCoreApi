using System;
using System.Collections.Generic;
using System.Text;

namespace Authorization
{
    public class MyAuthorizeAttribute : Attribute
    {
        private List<string> list = new List<string>();
        public MyAuthorizeAttribute(Type[] type)
        {
            foreach (var item in type)
            {
                list.Add(item.ToString());
            }
        }
        public MyAuthorizeAttribute(Type type)
        {
            list.Add(type.ToString());
        }

        /// <summary>
        /// 获取权限list
        /// </summary>
        /// <returns></returns>
        public List<string> GetStingList()
        {
            return list;
        }
    }

    /// <summary>
    /// 检验是否登录
    /// </summary>
    public class CheckLoginAttribute :Attribute{ }

    /// <summary>
    /// 不检验是否登录
    /// </summary>
    public class NotCheckLoginAttribute : Attribute { }
}
