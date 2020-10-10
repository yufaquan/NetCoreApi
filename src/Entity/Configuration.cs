using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Common;
using SqlSugar;

namespace Entity
{
    ///<summary>
    ///系统配置
    ///</summary>
    [YDisplay("系统配置")]
    [KnownType(typeof(Configuration))]
    public class Configuration
    {
        /// <summary>
        /// 附件上传大小限制（兆）
        /// 为0无限制
        /// </summary>
        [YDisplay("附件上传大小限制（兆）")]
        public int LimitUpFileSize { get; set; }

    }


    [SugarTable("sys_configuration")]
    public class SysConfiguration : BaseModel
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public string Description { get; set; }
    }
}
