using Common;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NetCoreAPI
{
    /// <summary>
    /// 全局异常处理
    /// </summary>
    public class ExceptionActionFilter : IExceptionFilter
    {
        #region Overrides of ExceptionFilterAttribute

        public  void OnException(ExceptionContext context)
        {
            var actionDescriptor = (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor;
            Type controllerType = actionDescriptor.ControllerTypeInfo;

            var controllerBase = typeof(ControllerBase);
            var controller = typeof(Controller);

            // Api's implements ControllerBase but not Controller
            if (controllerType.IsSubclassOf(controllerBase) && !controllerType.IsSubclassOf(controller))
            {
                // Handle web api exception
                //context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //context.HttpContext.Response.ContentType = "application/json";
                context.Result = new JsonResult(HttpResult.Error);
            }

            // Pages implements ControllerBase and Controller
            if (controllerType.IsSubclassOf(controllerBase) && controllerType.IsSubclassOf(controller))
            {
                // Handle page exception
            }



            //base.OnException(context);
            Current.Clear();
            context.ExceptionHandled = true;
        }

        #endregion

        /// <summary>
        /// 写入日志（log4net）
        /// </summary>
        /// <param name="context">提供使用</param>
        private void WriteLog(ExceptionContext context)
        {
            if (context == null)
                return;
            if (context.Exception is NoAuthorizeException || context.Exception is UserFriendlyException)
            {
                //友好错误提示,未授权错误提示，记录警告日志
                //Log.WriteError(context.Exception.Message);
            }
            else
            {
                //异常错误，
                //Log.WriteFatal(context.Exception.Message);

                ////TODO :写入错误日志到数据库
                ExceptionMessage error = new ExceptionMessage(context.Exception);
                //new Tally_API.Base.DBOther().Insert(error);
            }
        }
    }



    /// <summary>
    /// 没有被授权的异常
    /// </summary>
    public class NoAuthorizeException : Exception
    {
        public NoAuthorizeException(string message)
            : base(message)
        {
        }
    }
    /// <summary>
    /// 用户友好异常
    /// </summary>
    public class UserFriendlyException : Exception
    {
        public UserFriendlyException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// 异常错误信息
    /// </summary>
    [YDisplay("异常错误信息")]
    [Serializable]
    [SugarTable("sys_fatal_logs")]
    public class ExceptionMessage
    {
        public ExceptionMessage()
        {
        }

        /// <summary>
        /// 构造函数
        /// 默认显示异常页面
        /// </summary>
        /// <param name="ex">异常对象</param>
        public ExceptionMessage(Exception ex)
            : this(ex, true)
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="isShowException">是否显示异常页面</param>
        public ExceptionMessage(Exception ex, bool isShowException)
        {
            MsgType = ex.GetType().Name;
            Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            StackTrace = ex.StackTrace.Length > 300 ? ex.StackTrace.Substring(0, 300) : ex.StackTrace;
            Source = ex.Source;
            Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Assembly = ex.TargetSite.Module.Assembly.FullName;
            Method = ex.TargetSite.Name;

            ShowException = isShowException;
            //var request = HttpContext.Current.Request;
            //IP = UrlOper.GetWebClientIp();
            //UserAgent = request.UserAgent;
            //Path = request.Path;
            //HttpMethod = request.HttpMethod;
            //UserName = Sessions.CurrentUser.NickName;
        }
        public int Id { get; set; }
        public string UserName { get; set; }
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
        /// 是否显示异常界面
        /// </summary>
        public bool ShowException { get; set; }

        /// <summary>
        /// 异常发生时间
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 异常发生方法
        /// </summary>
        public string Method { get; set; }
    }

    /// <summary>
    /// 表示Ajax操作结果
    /// </summary>
    public class AjaxResult
    {
        /// <summary>
        /// 获取 Ajax操作结果类型
        /// </summary>
        public ResultType type { get; set; }

        /// <summary>
        /// 获取 Ajax操作结果编码
        /// </summary>
        public int errorcode { get; set; }

        /// <summary>
        /// 获取 消息内容
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 获取 返回数据
        /// </summary>
        public object resultdata { get; set; }
    }
    /// <summary>
    /// 表示 ajax 操作结果类型的枚举
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// 消息结果类型
        /// </summary>
        info = 0,

        /// <summary>
        /// 成功结果类型
        /// </summary>
        success = 1,

        /// <summary>
        /// 警告结果类型
        /// </summary>
        warning = 2,

        /// <summary>
        /// 异常结果类型
        /// </summary>
        error = 3
    }

}
