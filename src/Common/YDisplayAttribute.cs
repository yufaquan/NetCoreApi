using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    /// <summary>
    /// 自定义显示
    /// </summary>
    public class YDisplayAttribute : Attribute
    {
        string name;
        string description;
        bool requisite;
        public YDisplayAttribute(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get { return name; } }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get { return description; } set { description = value; } }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool Requisite
        {
            get
            {
                return requisite;
            }

            set
            {
                requisite = value;
            }
        }
    }
}
