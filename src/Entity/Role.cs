using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Common;
using SqlSugar;

namespace Entity
{
    ///<summary>
    ///角色
    ///</summary>
    [YDisplay("角色")]
    [SugarTable("sys_role")]
    [KnownType(typeof(Role))]
    public partial class Role : BaseModel
    {


        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int PId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>           
        public string Name { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>           
        public string Description { get; set; }
        /// <summary>
        /// 角色权限
        /// </summary>
        public string Permissions { get; set; }

    }
}
