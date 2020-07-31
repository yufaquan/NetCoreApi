using Entity;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Common
{
    public static class AttributeExtensions
    {
        public static string GetDisplayName(this BaseModel type)
        {
            var t = type.GetType();
            foreach (var attributes in t.GetCustomAttributes(false))
            {
                if (attributes.GetType() == typeof(YDisplayAttribute))
                {
                    YDisplayAttribute yd = (YDisplayAttribute)attributes;
                    return yd.Name;
                }
            }
            return null;
        }


        public static string GetDisplayName(this Type type)
        {
            var t = type;
            foreach (var attributes in t.GetCustomAttributes(false))
            {
                if (attributes.GetType() == typeof(YDisplayAttribute))
                {
                    YDisplayAttribute yd = (YDisplayAttribute)attributes;
                    return yd.Name;
                }
            }
            return null;
        }

        public static string GetDisplayName(this BaseModel type, string fildName)
        {
            Type t = type.GetType();
            foreach (PropertyInfo p in t.GetProperties())
            {
                if (p.Name == fildName)
                {
                    foreach (var attributes in p.GetCustomAttributes(false))
                    {
                        if (attributes.GetType() == typeof(YDisplayAttribute))
                        {
                            YDisplayAttribute yd = (YDisplayAttribute)attributes;
                            return yd.Name;
                        }
                    }
                }
            }
            return null;
        }





    }
}
