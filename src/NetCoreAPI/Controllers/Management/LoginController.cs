using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoginByPwd(string name,string pwd)
        {
            return new JsonResult(HttpResult.Success("a"));
        }
    }
}
