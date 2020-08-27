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
        /// <param name="name">帐号</param>
        /// <param name="pwd">密码(MD5加密一次后传入)</param>
        /// <returns></returns>
        [HttpPost]
        [NotCheckLogin]
        public JsonResult LoginByPwd(string name,string pwd)
        {
            string errorMessage;
            var token = UserBussiness.Init.LoginByPwd(name, pwd, out errorMessage);
            if (!string.IsNullOrWhiteSpace(token) && string.IsNullOrWhiteSpace(errorMessage))
            {
                return new JsonResult(HttpResult.Success(new { token }));
            }
            else
            {
                return new JsonResult(HttpResult.Success(HttpResultCode.LoginFail, errorMessage, null));
            }
        }
    }
}
