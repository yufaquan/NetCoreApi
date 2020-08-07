using Common;
using Microsoft.Extensions.Logging;
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
    [SugarTable("sys_log_api")]
    public class LogAPI
    {
        public int Id { get; set; }
        /// <summary>
        /// 响应开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 响应结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 请求路径
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 请求类型
        /// </summary>
        public string HttpMethod { get; set; }
        /// <summary>
        /// 日志等级
        /// </summary>
        public Enums.LogLevel LogLevel { get; set; }
        /// <summary>
        /// 响应时长（秒）
        /// </summary>
        public double ElapsedTime { get; set; }
        /// <summary>
        /// 请求参数
        /// </summary>
        public string RequestParameter { get; set; }
        /// <summary>
        /// 返回参数
        /// </summary>
        public string ResponseParameter { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ExceptionInfo { get; set; }
        /// <summary>
        /// 用户IP
        /// </summary>
        public string UserIP { get; set; }
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string ServiceIP { get; set; }
        /// <summary>
        /// 浏览器信息
        /// </summary>
        public string BrowserType { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }
    }
}
