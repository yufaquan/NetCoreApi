using Common;
using Microsoft.AspNetCore.Http;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    /// <summary>
    /// 异常错误信息
    /// </summary>
    [YDisplay("异常错误信息")]
    [Serializable]
    [SugarTable("sys_log_error")]
    public class LogError
    {
        public LogError()
        {
        }
        public LogError(Exception ex, HttpContext httpContext)
        {
            MsgType = ex.GetType().Name;
            Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            StackTrace = ex.StackTrace.Length > 300 ? ex.StackTrace.Substring(0, 300) : ex.StackTrace;
            Source = ex.Source;
            LogLevel = Enums.LogLevel.Error;
            Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Assembly = ex.TargetSite.Module.Assembly.FullName;
            Method = ex.TargetSite.Name;

            if (httpContext!=null)
            {
                try
                {
                    HttpRequest request = httpContext.Request;
                    //HttpContextAccessor icontext = new HttpContextAccessor();
                    IP = httpContext?.Connection.RemoteIpAddress.ToString();
                    UserAgent = request.Headers["User-Agent"];
                    Path = request.Path;
                    HttpMethod = request.Method;
                }
                catch (Exception)
                {

                }
            }
            UserName = Current.UserJson.JsonToModelOrDefault<User>().Name;
        }
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 日志等级
        /// </summary>
        public Enums.LogLevel LogLevel { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public string MsgType { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 请求路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 程序集名称
        /// </summary>
        public string Assembly { get; set; }

        /// <summary>
        /// 异常参数
        /// </summary>
        public string ActionArguments { get; set; }

        /// <summary>
        /// 请求类型
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// 异常堆栈
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// 异常源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 服务器IP 端口
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 客户端浏览器标识
        /// </summary>
        public string UserAgent { get; set; }



        /// <summary>
        /// 异常发生时间
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 异常发生方法
        /// </summary>
        public string Method { get; set; }
    }

}
