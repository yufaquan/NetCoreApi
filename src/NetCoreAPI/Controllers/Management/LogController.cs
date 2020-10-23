using System.Collections.Generic;
using System.Linq;
using Authorization;
using Bussiness;
using Bussiness.Mangement;
using Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace NetCoreAPI.Controllers.Management
{
    /// <summary>
    /// 日志
    /// </summary>
    [ApiController]
    public class LogController : ManagementApiController
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        public LogController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            Current.ServerPath = _webHostEnvironment.ContentRootPath;
        }

        /// <summary>
        /// 获取API访问列表
        /// </summary>
        /// <param name="from">筛选条件:访问来源</param>
        /// <param name="page">页数</param>
        /// <param name="limit">每页多少条</param>
        /// <returns></returns>
        [MyAuthorize(typeof(Read<LogAPI>))]
        [HttpGet]
        public JsonResult GetAPIList(string from, int page, int limit)
        {
            var total = 0;
            var data = new LogAPI();
            data.From = from;
            var list = LogBussiness.Init.GetAPIPageList(data, page, limit, ref total);
            //string errorMessage;
            var result = new {
                total,
                page,
                limit,
                list = from a in list select new
                {
                    a.Id,a.BrowserType,a.ElapsedTime,a.ExceptionInfo,a.From,a.HttpMethod,a.LogLevel,
                    a.RequestParameter,a.ResponseParameter,a.ServiceIP,a.Url,a.UserIP,a.UserName,
                    LogLevelName = a.LogLevel.GetDisplayName(),
                    EndTime = a.EndTime.ToLongString(),
                    StartTime = a.StartTime.ToLongString()
                }
            };
            return new JsonResult(HttpResult.Success(result));
        }

        /// <summary>
        /// 获取操作日志
        /// </summary>
        /// <param name="content">筛选条件:操作内容</param>
        /// <param name="page">页数</param>
        /// <param name="limit">每页多少条</param>
        /// <returns></returns>
        [MyAuthorize(typeof(Read<LogEvent>))]
        [HttpGet]
        public JsonResult GetEventList(string content, int page, int limit)
        {
            var total = 0;
            var data = new LogEvent();
            data.Content = content;
            var list = LogBussiness.Init.GetEventPageList(data, page, limit, ref total);
            //string errorMessage;
            var result = new
            {
                total,
                page,
                limit,
                list = from a in list
                       select new
                       {
                           a.Id,a.Content,a.EventType,a.Remark,a.UserName,
                           EventTypeName=a.EventType.GetDisplayName(),
                           WriteDate = a.WriteDate.ToLongString()
                       }
            };
            return new JsonResult(HttpResult.Success(result));
        }

        /// <summary>
        /// 获取错误日志
        /// </summary>
        /// <param name="userName">筛选条件:用户名称</param>
        /// <param name="page">页数</param>
        /// <param name="limit">每页多少条</param>
        /// <returns></returns>
        [MyAuthorize(typeof(Read<LogError>))]
        [HttpGet]
        public JsonResult GetErrorList(string userName, int page, int limit)
        {
            var total = 0;
            var data = new LogError();
            data.UserName = userName;
            var list = LogBussiness.Init.GetErrorPageList(data, page, limit, ref total);
            //string errorMessage;
            var result = new
            {
                total,
                page,
                limit,
                list = from a in list
                       select new
                       {
                           a.Id,a.ActionArguments,a.Assembly,a.HttpMethod,a.IP,a.LogLevel,a.Message,a.Method,a.MsgType,a.Path,a.Source,a.StackTrace,a.Time,a.UserAgent,a.UserName,
                           LogLevelName=a.LogLevel.GetDisplayName()
                       }
            };
            return new JsonResult(HttpResult.Success(result));
        }

    }
}
