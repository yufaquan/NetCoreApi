using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Common;
using SqlSugar;

namespace Entity
{
    /// <summary>
    /// 菜单
    /// </summary>
    /// 
    [YDisplay("菜单")]
    [SugarTable("sys_menu")]
    public class Menus:BaseModel
    {
        public Menus() { }
        public Menus(string name,string url)
        {
            Name = name;
            Url = url;
            Hide = false;
        }
        /// <summary>
        /// 父菜单Id
        /// </summary>
        public int PId { get; set; }
        /// <summary>
        /// 菜单标题
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 菜单路径 
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool Hide { get; set; }
        /// <summary>
        /// 访问菜单所需权限
        /// </summary>
        public List<string> Permissions { get; set; }

        /// <summary>
        /// 子菜单
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<Menus> SubMenu { get; set; }
        /// <summary>
        /// 菜单描述
        /// </summary>
        public string description { get; set; }

    }

}