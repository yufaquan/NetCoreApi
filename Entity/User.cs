using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Common;
using SqlSugar;

namespace Entity
{
    ///<summary>
    ///用户表
    ///</summary>
    [YDisplay("用户表")]
    [SugarTable("sys_user")]
    [KnownType(typeof(User))]
    public partial class User : BaseModel
    {
        public User()
        {


        }
        /// <summary>
        /// Desc:名称
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string Name { get; set; }

        /// <summary>
        /// Desc:昵称
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string NickName { get; set; }

        /// <summary>
        /// Desc:密码（md5加密两次）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PassWordMD5 { get; set; }

        /// <summary>
        /// Desc:手机号码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string Mobile { get; set; }

        /// <summary>
        /// Desc:邮箱
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string Email { get; set; }

        /// <summary>
        /// Desc:用户头像地址
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string HeadImgUrl { get; set; }

        /// <summary>
        /// 个性签名
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Desc:微信开放平台唯一Id
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string UnionId { get; set; }
        public string OpenIds { get; set; }

        /// <summary>
        /// Desc:性别 1男 2女 0未知
        /// Default:未知
        /// Nullable:False
        /// </summary>           
        public Enums.Sex Sex { get; set; }

        /// <summary>
        /// Desc:所在地区； 中国-湖北-武汉
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string Area { get; set; }

        /// <summary>
        /// Desc:国家id
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string CountryId { get; set; }

        /// <summary>
        /// Desc:省份
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string ProvinceId { get; set; }

        /// <summary>
        /// Desc:城市
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string CityId { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DistrictId { get; set; }

        /// <summary>
        /// 拥有的权限
        /// </summary>
        public string Permissions { get; set; }

        /// <summary>
        /// 所属的角色
        /// </summary>
        public string RoleIds { get; set; }
    }
}
