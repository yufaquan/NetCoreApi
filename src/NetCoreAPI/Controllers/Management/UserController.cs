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
    /// 菜单
    /// </summary>
    [ApiController]
    public class UserController : ManagementApiController
    {

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="name">筛选条件And拼接</param>
        /// <param name="pageIndex">页数</param>
        /// <param name="pageSize">每页多少条</param>
        /// <returns></returns>
        [MyAuthorize(typeof(Read<User>))]
        [HttpGet]
        public JsonResult GetList(string name,int pageIndex,int pageSize)
        {
            var pageCount = 0;
            var user = new User();
            user.Name = name;
            var list = UserBussiness.Init.GetPageList(user, pageIndex, pageSize, ref pageCount);
            string errorMessage;
            var result = new {
                pageCount,
                pageIndex,
                pageSize,
                list = from a in list select new
                {
                    a.Name, a.Id, a.Area, a.Email, a.HeadImgUrl, a.Mobile, a.NickName, Sex=a.Sex.GetDisplayName(),
                    CreatedAt = a.CreatedAt.ToLongString(),
                    CreatedBy = a.CreatedBy.HasValue ? ServiceHelp.GetUserService.GetById(a.CreatedBy.Value)?.Name : "",
                    RoleIds = (from role in RoleBussiness.Init.GetRoleByUserId(a.Id, out errorMessage) select new { role.Id, role.Name })
                }
            };
            return new JsonResult(HttpResult.Success(result));
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="user">用户</param>
        /// <returns></returns>
        [HttpPost]
        [MyAuthorize(typeof(Create<User>))]
        public JsonResult Add([FromBody] User user)
        {
            var result = ServiceHelp.GetUserService.Add(user);
            if (result==null)
            {
                return new JsonResult(HttpResult.Success( HttpResultCode.AddFail,result));
            }
            else
            {
                return new JsonResult(HttpResult.Success(result));
            }
        }
    }
}
