using Bussiness;
using Common;
using Entity;
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
using System.Threading.Tasks;

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
               
            }

            // Pages implements ControllerBase and Controller
            if (controllerType.IsSubclassOf(controllerBase) && controllerType.IsSubclassOf(controller))
            {
                // Handle page exception
            }

            context.Result = new JsonResult(HttpResult.Error);
            WriteLog(context);
            Current.Clear();
            context.ExceptionHandled = true;
        }

        #endregion

        /// <summary>
        /// 写入日志
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
                LogError error = new LogError();
                Exception ex = context.Exception;
                error.MsgType = ex.GetType().Name;
                error.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                error.StackTrace = ex.StackTrace.Length > 300 ? ex.StackTrace.Substring(0, 300) : ex.StackTrace;
                error.Source = ex.Source;
                error.LogLevel = Enums.LogLevel.Error;
                error.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                error.Assembly = ex.TargetSite.Module.Assembly.FullName;
                error.Method = ex.TargetSite.Name;
                
                HttpRequest request = context.HttpContext.Request;
                //HttpContextAccessor icontext = new HttpContextAccessor();
                error.IP = context.HttpContext?.Connection.RemoteIpAddress.ToString();
                error.UserAgent = request.Headers["User-Agent"];
                error.Path = request.Path;
                error.HttpMethod = request.Method;
                error.UserName = Current.UserJson.JsonToModelOrDefault<User>().Name;
                var task = ServiceHelp.GetLogService.WriteErrorLogAsync(error);
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

    

}
