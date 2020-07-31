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

        public List<string> GetStingList()
        {
            return list;
        }
    }
}
