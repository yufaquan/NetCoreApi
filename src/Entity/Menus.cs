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
        /// <summary>
        /// 父菜单Id
        /// </summary>
        public int PId { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 重定向
        /// </summary>
        public string Redirect { get=>Children!=null&&Children.Count>0?Children.FirstOrDefault().Path: Redirect; set=>Redirect=value; }

        /// <summary>
        /// 虚拟路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 菜单真实路径 
        /// </summary>
        public string Component { get=>string.IsNullOrWhiteSpace(Component)? "Layout" : Component; set=>Component=value; }
        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool Hide { get; set; }
        /// <summary>
        /// 访问菜单所需权限
        /// </summary>
        public string Permissions { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 子菜单
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<Menus> Children { get; set; }
        /// <summary>
        /// 菜单描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否一直显示
        /// </summary>
        public bool AlwaysShow { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        [SugarColumn(IsIgnore = true)]
        public Meta Meta { get => new Meta() {Title=Title, Icon=Icon }; }
    }

    public class Meta
    {
        public string Title { get; set; }
        public string Icon { get; set; }
    }

}