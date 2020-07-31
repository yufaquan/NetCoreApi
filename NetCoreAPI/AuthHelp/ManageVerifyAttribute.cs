using Authorization;
using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            var headers = context.HttpContext.Request.Headers;
            //检测是否包含'Authorization'请求头，如果不包含返回context进行下一个中间件，用于访问不需要认证的API
            if (!headers.ContainsKey("Authorization"))
            {
               
            }
            var tokenStr = headers["Authorization"];
            string jwtStr = tokenStr.ToString().Substring("Bearer ".Length).Trim();
            //获得Controller类型
            Type t = context.Controller.GetType();
            //获得方法名
            string actionname = context.RouteData.Values["action"].ToString();
            //是否有权限
            if (!IsHaveAuthorize(Commons.FirstCharToUpper(actionname), t))
            {
                //context.HttpContext.Response.Clear();
                //context.HttpContext.AuthFailed();
                //记录访问

                context.Result = new JsonResult(HttpResult.NotAuth);
            }
            #endregion

        }

        /// <summary>
        /// 方法执行后
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            #region 记录API调用及响应时长等
            var stopwach = context.RouteData.Values[DURATION] as Stopwatch;
            stopwach.Stop();
            TimeSpan time = stopwach.Elapsed; 
            //记录访问

            #endregion
        }


        /// <summary>
        /// 判断是否有权限
        /// </summary>
        /// <param name="actionname"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool IsHaveAuthorize(string actionname, Type t)
        {
            //var upList = UserManagement.Initialize.GetCurrentUserPermissions();
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
            ////判断是否有权限
            foreach (var item in attrs)
            {
                var authorizeList = item.GetStingList();
                foreach (var authorize in authorizeList)
                {
                    //if (upList.Contains(authorize))
                    //{
                    //    return true;
                    //}
                }
            }
            return false;
        }
    }
}
