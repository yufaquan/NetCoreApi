using Authorization;
using Bussiness;
using Bussiness.Mangement;
using Common;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreAPI.AuthHelp
{
    /// <summary>
    /// 该过虑器提供给所有对外的RestApi接口使用
    /// 在有需要验证每个接口/方法是否只为管理人员用户调用时使用
    /// 方法级别的过率器
    /// </summary>
    public class ManageVerifyAttribute : Attribute, IActionFilter
    {
        private const string DURATION = "DURATION";


        /// <summary>
        /// 方法执行前
        /// 判断用户是否为管理角色，不是则抛出异常
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            #region 记录API调用及响应时长等
            var stopwach = new Stopwatch();
            stopwach.Start();
            context.RouteData.Values.Add(DURATION, stopwach);
            #endregion

            #region 权限验证
            //获得Controller类型
            Type t = context.Controller.GetType();
            //获得方法名
            string actionname = context.RouteData.Values["action"].ToString();

            var headers = context.HttpContext.Request.Headers;

            string errorMessage = string.Empty;

            //判断师傅需要检查访问token和UserToken
            if (IsHaveVisitToken(actionname, t))
            {
                #region 访问token的校验
                //检测是否包含'Authorization'请求头，如果不包含返回context进行下一个中间件，用于访问不需要认证的API
                if (!headers.ContainsKey("Authorization"))
                {
                    context.Result = new JsonResult(HttpResult.NotAuth);
                    return;
                }
                var tokenStr = headers["Authorization"];
                string jwtStr = tokenStr.ToString().Substring("Bearer ".Length).Trim();
                if (!TokenHelp.VerifyVisitToken(jwtStr, false, out errorMessage))
                {
                    context.Result = new JsonResult(HttpResult.NotAuth);
                    return;
                }
                #endregion

            }

            #region 用户token的校验
            //存储当前访问的用户token
            if (headers.ContainsKey("UserToken"))
            {
                Current.UserToken = headers["UserToken"];
            }
            else
            {
                Current.UserToken = string.Empty;
            }

            //是否有权限
            if (!IsHaveAuthorize(actionname, t, out errorMessage))
            {
                context.Result = new JsonResult(HttpResult.NotAuth);
                return;
            }
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                context.Result = new JsonResult(HttpResult.UserTokenError(new { }, errorMessage + "请重新登录。"));
            }
            #endregion
            //成功访问
            #endregion

        }

        /// <summary>
        /// 方法执行后
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            #region 记录API调用及响应时长等
            Stopwatch stopwach = context.RouteData.Values[DURATION] as Stopwatch;
            stopwach.Stop();
            var dt = DateTime.Now;
            TimeSpan time = stopwach.Elapsed;
             //记录访问
             var a = time.TotalSeconds;

            #region 异步记录api访问
            LogAPI log = new LogAPI();
            HttpRequest request = context.HttpContext.Request;
            log.BrowserType = request.Headers["User-Agent"]; 
            log.ElapsedTime = a;
            log.EndTime = dt;
            log.ExceptionInfo = context.Exception?.Message;
            log.HttpMethod= request.Method;
            log.LogLevel = context.Exception!=null? Enums.LogLevel.Error: Enums.LogLevel.Info;
            StringBuilder rp = new StringBuilder();
            #region 获取请求参数
            if (request.Method.AsStrs(new string[] {"put","post" }))
            {
                try
                {
                    if (request.Form != null && request.Form.Count > 0)
                    {
                        rp.Append("{");
                        foreach (var item in request.Form.Keys)
                        {
                            rp.Append($"\"{item}\":\"{request.Form[item]}\",");
                        }
                        rp.Append("}");
                    }
                }
                catch (Exception)
                {

                }
            }
            else if (request.Query != null && request.Query.Count > 0)
            {
                rp.Append("{");
                foreach (var item in request.Query.Keys)
                {
                    rp.Append($"\"{item}\":\"{request.Form[item]}\",");
                }
                rp.Append("}");
            } 
            #endregion
            log.RequestParameter = rp.ToString().Replace(",}", "}");
            log.ResponseParameter = context.Result == null ? "" : context.Result.ToJsonString();
            log.ServiceIP = request.HttpContext.Connection.LocalIpAddress.MapToIPv4().ToString() + ":" + request.HttpContext.Connection.LocalPort;
            log.StartTime = dt - time;
            log.Url = request.Path.ToUriComponent();
            log.UserId = Current.UserId.HasValue?Current.UserId.Value:0;
            log.UserIP = context.HttpContext?.Connection.RemoteIpAddress.ToString();
            log.UserName= Current.UserJson.JsonToModelOrDefault<User>().Name;
            Task task= ServiceHelp.GetLogService.WriteApiLogAsync(log);
            #endregion
            #endregion

            if (context.Exception==null)
            {
                Current.Clear();
            }
        }


        /// <summary>
        /// 判断是否需要访问令牌
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="t"></param>
        /// <returns>true：需要；</returns>
        private bool IsHaveVisitToken(string actionName, Type t)
        {
            actionName = Commons.FirstCharToUpper(actionName);
            var method = t.GetMethod(actionName);
            if (method == null)
            {
                return false;
            }
            ////查看是否需要访问令牌
            var attrs = method.GetCustomAttributes(typeof(AllowAnonymousAttribute), true) as AllowAnonymousAttribute[];
            if (attrs.Length == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 判断是否有权限
        /// </summary>
        /// <param name="actionname">方法名</param>
        /// <param name="t">Controller类型</param>
        /// <param name="errorMessage">Controller类型</param>
        /// <returns>true：有权限；</returns>
        private bool IsHaveAuthorize(string actionname, Type t, out string errorMessage)
        {
            actionname = Commons.FirstCharToUpper(actionname);
            errorMessage = string.Empty;
            var method = t.GetMethod(actionname);
            if (method == null)
            {
                return false;
            }
            ////查看是否需要权限控制
            var attrs = method.GetCustomAttributes(typeof(MyAuthorizeAttribute), true) as MyAuthorizeAttribute[];
            if (attrs.Length == 0)
            {
                return true;
            }

            //检验用户token
            int? userId;
            if (!TokenHelp.VerifyUserToken(Current.UserToken, out userId, out errorMessage))
            {
                return false;
            }
            //记录当前用户Id
            Current.UserId = userId;
            //未获取到当前用户
            if (!Current.UserId.HasValue || (Current.UserId.HasValue && Current.UserId.Value == 0))
            {
                return false;
            }
            //获取用户权限
            var user = ServiceHelp.GetUserService.GetById(Current.UserId.Value);
            Current.UserJson = user.ToJsonString();
            if (user==null)
            {
                return false;
            }
            var ups = Commons.Split(user.Permissions, ',');
            //系统管理员角色和超级管理员拥有全部权限
            if (user.Id==1 || user.RoleIds.ToList(',').Exists(x=>x=="1"))
            {
                return true;
            }
            //获取用户角色的权限
            List<string> rlist = RoleBussiness.Init.GetRolePermissionsByUserId(user.Id,out errorMessage);

            var permissions = ups.Union(rlist);

            ////判断是否有权限
            foreach (var item in attrs)
            {
                var authorizeList = item.GetStingList();
                foreach (var authorize in authorizeList)
                {
                    if (permissions.Contains(authorize))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
