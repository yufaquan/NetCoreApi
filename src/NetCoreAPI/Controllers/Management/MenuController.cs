using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Authorization;
using Bussiness;
using Bussiness.Mangement;
using Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreAPI.Controllers.Management
{
    /// <summary>
    /// 菜单
    /// </summary>
    [ApiController]
    public class MenuController : ManagementApiController
    {
        /// <summary>
        /// 获取所有菜单
        /// </summary>
        /// <returns></returns>
        [MyAuthorize(typeof(Read<Menus>))]
        [HttpGet]
        public JsonResult GetAllList()
        {
            var list = ServiceHelp.GetMenuService.GetAllList(null);
            list = list == null ? new List<Menus>() : list;
            return new JsonResult(HttpResult.Success(new { list = list }));
        }

        /// <summary>
        /// 获取ElementUI所需的菜单列表
        /// </summary>
        /// <returns></returns>
        [MyAuthorize(typeof(Read<Menus>))]
        [HttpGet]
        public JsonResult GetAllElementList()
        {
            var list = MenusBussiness.Init.GetAllElementList();
            list = list == null ? new List<Menus>() : list;
            return new JsonResult(HttpResult.Success(new { list = list }));
        }


        /// <summary>
        /// 获取有权限的ElementUI所需的菜单路由
        /// </summary>
        /// <returns></returns>
        [MyAuthorize(typeof(Read<Menus>))]
        [HttpGet]
        public JsonResult GetHavePermissionsElementList()
        {
            var list = MenusBussiness.Init.GetHavePermissionsElementList();
            return new JsonResult(HttpResult.Success(new { list = list }));
        }


        /// <summary>
        /// 获取当前用户有权限的菜单在前端进行展示
        /// </summary>
        /// <returns></returns>
        [MyAuthorize(typeof(Read<Menus>))]
        [HttpGet]
        public JsonResult GetHavePermissionsList()
        {
            var list = MenusBussiness.Init.GetHavePermissionsList();
            list = list == null ? new List<Menus>() : list;
            return new JsonResult(HttpResult.Success(new { list = list }));
        }

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="menus">菜单</param>
        /// <returns></returns>
        [HttpPost]
        [MyAuthorize(typeof(Create<Menus>))]
        public JsonResult Add([FromBody] Menus menus)
        {
            var result = ServiceHelp.GetMenuService.Add(menus);
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
