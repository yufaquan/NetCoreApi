using Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    ///<summary>
    ///api访问记录表
    ///</summary>
    [YDisplay("用户表")]
    [SugarTable("sys_log_event")]
    public class LogEvent
    {

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime WriteDate { get; set; }
        /// <summary>
        /// 操作人id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 操作人名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public Enums.EventType EventType { get; set; }
        /// <summary>
        /// 操作内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
