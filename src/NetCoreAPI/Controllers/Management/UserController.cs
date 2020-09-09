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
    /// 用户
    /// </summary>
    [ApiController]
    public class UserController : ManagementApiController
    {

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="name">筛选条件And拼接</param>
        /// <param name="page">页数</param>
        /// <param name="limit">每页多少条</param>
        /// <returns></returns>
        [MyAuthorize(typeof(Read<User>))]
        [HttpGet]
        public JsonResult GetList(string name,int page,int limit)
        {
            var pageCount = 0;
            var user = new User();
            user.Name = name;
            var list = UserBussiness.Init.GetPageList(user, page, limit, ref pageCount);
            string errorMessage;
            var result = new {
                pageCount,
                page,
                limit,
                list = from a in list select new
                {
                    a.Name, a.Id, a.Area, a.Email, a.HeadImgUrl, a.Mobile, a.NickName,
                    a.Sex,
                    CreatedAt = a.CreatedAt.ToLongString(),
                    CreatedBy = a.CreatedBy.HasValue ? ServiceHelp.GetUserService.GetById(a.CreatedBy.Value)?.Name : "",
                    Roles = (from role in RoleBussiness.Init.GetRoleByUserId(a.Id, out errorMessage) select new { role.Id, role.Name })
                }
            };
            return new JsonResult(HttpResult.Success(result));
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [MyAuthorize(typeof(Read<User>))]
        public JsonResult Info()
        {
            if (Current.UserId.HasValue)
            {
                string errorMessage;
                var user = ServiceHelp.GetUserService.GetById(Current.UserId.Value);
                var result = new {
                    user.Area,user.CityId,user.CountryId,user.DistrictId,user.Email,user.HeadImgUrl,user.Id,user.Mobile,user.Name,user.NickName,user.ProvinceId,user.Signature,
                    Sex=user.Sex.GetDisplayName(),
                    Permissions =user.Permissions.ToList(','),
                    Roles= (from role in RoleBussiness.Init.GetRoleByUserId(user.Id, out errorMessage) select new { role.Id, role.Name }),
                    CreatedBy = user.CreatedBy.HasValue ? ServiceHelp.GetUserService.GetById(user.CreatedBy.Value)?.Name : "",
                    CreatedAt =user.CreatedAt.ToLongString()
                };
                return new JsonResult(HttpResult.Success(result));
            }
            else
            {
                return new JsonResult(HttpResult.Success(HttpResultCode.SelectFail,"未获取到用户信息。",null));
            }
            
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
            string errorMessage;
            var result = UserBussiness.Init.Add(user,out errorMessage);
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
