using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using Authorization;
using Bussiness;
using Bussiness.Mangement;
using Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Common;

namespace NetCoreAPI.Controllers.Management
{
    /// <summary>
    /// 角色
    /// </summary>
    [ApiController]
    public class RoleController : ManagementApiController
    {

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="name">筛选条件And拼接</param>
        /// <param name="page">页数</param>
        /// <param name="limit">每页多少条</param>
        /// <returns></returns>
        [MyAuthorize(typeof(Read<Role>))]
        [HttpGet]
        public JsonResult GetList(string name,int page,int limit)
        {
            var total = 0;
            var data = new Role();
            data.Name = name;
            var list = RoleBussiness.Init.GetPageList(data, page, limit, ref total);
            string errorMessage;
            var result = new {
                total,
                page,
                limit,
                list = from a in list select new
                {
                    a.Name, a.Id, a.Description,a.Permissions,a.PId,
                    CreatedAt = a.CreatedAt.ToLongString(),
                    CreatedBy = a.CreatedBy.HasValue ? ServiceHelp.GetRoleService.GetById(a.CreatedBy.Value)?.Name : ""
                }
            };
            return new JsonResult(HttpResult.Success(result));
        }


        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="data">角色</param>
        /// <returns></returns>
        [HttpPost]
        [MyAuthorize(typeof(Create<Role>))]
        public JsonResult Add([FromBody] Role data)
        {
            string errorMessage;
            var result = RoleBussiness.Init.Add(data, out errorMessage);
            if (result==null)
            {
                return new JsonResult(HttpResult.Success( HttpResultCode.AddFail,errorMessage,result));
            }
            else
            {
                return new JsonResult(HttpResult.Success(result));
            }
        }
    }
}
