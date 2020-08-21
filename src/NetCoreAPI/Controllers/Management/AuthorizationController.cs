using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreAPI.Controllers.Management
{
    /// <summary>
    /// 权限
    /// </summary>
    public class AuthorizationController : ManagementApiController
    {
        /// <summary>
        /// 获取所有权限字典
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAllAuthorization()
        {
            return new JsonResult(HttpResult.Success(AuthorizeData.GetTextAndStringSecurity()));
        }

        /// <summary>
        /// 获取所有权限值
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAllAuthorizationStringList()
        {
            return new JsonResult(HttpResult.Success(AuthorizeData.GetAuthorizeStringListSecurity()));
        }
    }
}
