using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Common;
using SqlSugar;
using static Entity.Enums;

namespace Entity
{
    ///<summary>
    ///用户扩展信息
    ///</summary>
    [YDisplay("用户扩展信息")]
    [SugarTable("sys_userinfo")]
    [KnownType(typeof(UserInfo))]
    public partial class UserInfo : BaseModel
    {


        /// <summary>
        /// 用户Id
        /// </summary>           
        public int UserId { get; set; }

        /// <summary>
        /// 项目唯一标识
        /// </summary>           
        public string OpenId { get; set; }

        /// <summary>
        /// 项目id
        /// </summary>           
        public int ProjectId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>           
        public ProjectType ProjectType { get; set; }

    }
}
