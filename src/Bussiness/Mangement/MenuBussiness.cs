using Authorization;
using Common;
using Entity;
using IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Mangement
{
    public class MenusBussiness
    {
        private IMenuService _service;
        public MenusBussiness()
        {
            _service = ServiceHelp.GetMenuService;
        }
        public static MenusBussiness Init { get => new MenusBussiness(); }

        /// <summary>
        /// 获取拥有权限的菜单
        /// </summary>
        /// <returns></returns>
        public List<Menus> GetHavePermissionsList()
        {
            string errorMessage;
            bool isAdmin;
            List<string> allP = UserBussiness.Init.GetAllPermission(null,out isAdmin,out errorMessage);
            if (isAdmin)
            {
                return ServiceHelp.GetMenuService.GetAllList(null).ToList();
            }
            else if(allP !=null && allP.Count>0)
            {
                return ServiceHelp.GetMenuService.GetAllList(x=>allP.IsHaveSame(x.Permissions.ToList(','))).ToList();
            }
            return new List<Menus>();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="menus"></param>
        /// <param name="errorMessage"></param>
        /// <returns>Null：失败；</returns>
        public Menus Add(Menus menus, out string errorMessage)
        {
            if (!VerifyData(menus, out errorMessage))
            {
                return null;
            }
            var rMenus = _service.Add(menus);
            if (rMenus == null)
            {
                errorMessage = "创建失败。";
                return null;
            }
            //记录操作日志
            Task task = ServiceHelp.GetLogService.WriteEventLogCreateAsync(typeof(Menus), rMenus.Id, rMenus.ToJsonString());
            return rMenus;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="menus"></param>
        /// <param name="errorMessage"></param>
        /// <returns>Null：失败；</returns>
        public Menus Edit(Menus menus, out string errorMessage)
        {
            if (!VerifyData(menus, out errorMessage))
            {
                return null;
            }
            if (_service.GetById(menus.Id) == null)
            {
                errorMessage = "未查询到数据！";
                return null;
            }
            var rMenus = _service.Edit(menus);
            if (rMenus == null)
            {
                errorMessage = "修改失败。";
                return null;
            }
            //记录操作日志
            Task task = ServiceHelp.GetLogService.WriteEventLogEditAsync(typeof(Menus), rMenus.Id, rMenus.ToJsonString());
            return rMenus;
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="errorMessage"></param>
        /// <returns>True：成功；</returns>
        public bool Delete(int Id, out string errorMessage)
        {
            errorMessage = string.Empty;
            var rb = _service.DeleteById(Id);
            if (rb)
            {
                //删除成功，记录日志
                Task task = ServiceHelp.GetLogService.WriteEventLogDeleteAsync(typeof(Menus), Id);
            }
            return rb;
        }

        /// <summary>
        /// 检测数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns>True：校验通过；</returns>
        private bool VerifyData(Menus data, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (data == null)
            {
                errorMessage = "请检查是否传入数据。";
                return false;
            }

            if (_service.GetAllList(x => x.Name == data.Name).Count > 0)
            {
                errorMessage = "角色名称已存在。";
                return false;
            }
            if (data.PId != 0 && _service.GetById(data.PId) == null)
            {
                errorMessage = "未找到上级。";
                return false;
            }
            return true;
        }
    }
}
