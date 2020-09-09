using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreAPI.Controllers.Portal
{
    [Route("[controller]")]
    [ApiController]
    public class GetVisitTokenController : ControllerBase
    {

        /// <summary>
        /// 获取访问token
        /// JWT授权(数据将在请求头中进行传输) 参数结构: "Authorization: Bearer {token}"
        /// </summary>
        /// <param name="key">唯一标识</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Get(string key)
        {
            string token = TokenHelp.WriteVisitToken(key);
            return new JsonResult(HttpResult.Success(new { Token = token }));
        }
    }
}
