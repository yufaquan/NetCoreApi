using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Authorization;
using Bussiness.Mangement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreAPI.Controllers.Management
{
    /// <summary>
    /// 登录
    /// </summary>
    public class LoginController : ManagementApiController
    {
        /// <summary>
        /// 帐号密码登录
        /// </summary>
        /// <param name="info">登录参数</param>
        /// <returns></returns>
        [HttpPost]
        [NotCheckLogin]
        public JsonResult LoginByPwd([FromBody] LoginInfo info)
        {
            string errorMessage;
            var token = UserBussiness.Init.LoginByPwd(info.Name, info.Pwd.ToUpper(), out errorMessage);
            if (!string.IsNullOrWhiteSpace(token) && string.IsNullOrWhiteSpace(errorMessage))
            {
                return new JsonResult(HttpResult.Success(new { token }));
            }
            else
            {
                return new JsonResult(HttpResult.Success(HttpResultCode.LoginFail, errorMessage, null));
            }
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult LoginOut()
        {
            if (UserBussiness.Init.LoginOut())
            {
                return new JsonResult(HttpResult.Success(null));
            }
            else
            {
                return new JsonResult(HttpResult.Success(HttpResultCode.Other, "登出失败，请稍候重试！", null));
            }
        }
    }
    public class LoginInfo
    {
        /// <summary>
        /// 用户名/电话/邮箱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 密码(MD5加密一次后传入)
        /// </summary>
        public string Pwd { get; set; }
    }
}
