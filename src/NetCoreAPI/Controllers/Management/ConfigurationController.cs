using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Bussiness.Mangement;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Authorization;

namespace NetCoreAPI.Controllers.Management
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class ConfigurationController : ManagementApiController
    {
        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [MyAuthorize(typeof(Read<Configuration>))]
        public JsonResult Get()
        {
            return new JsonResult(HttpResult.Success(ConfigurationBussiness.Init.Get()));
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [MyAuthorize(typeof(Update<Configuration>))]
        public JsonResult Set([FromBody] Configuration configuration)
        {
            bool isSuccess= ConfigurationBussiness.Init.Set(configuration,HttpContext);
            if (isSuccess)
            {
                return new JsonResult(HttpResult.Success(null));
            }
            else
            {
                return new JsonResult(HttpResult.Success(HttpResultCode.EditFail,null));
            }
        }
    }
}
