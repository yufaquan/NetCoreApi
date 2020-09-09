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
   public class RoleBussiness
    {
        private IRoleService _service;
        public RoleBussiness()
        {
            _service = ServiceHelp.GetRoleService;
        }
        public static RoleBussiness Init { get => new RoleBussiness(); }

        /// <summary>
        /// 根据用户id 获取角色
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public List<Role> GetRoleByUserId(int userId,out string errorMessage)
        {
            errorMessage = string.Empty;
            User u = ServiceHelp.GetUserService.GetById(userId);
            if (u==null)
            {
                errorMessage = "未查询到用户。";
                return new List<Role>();
            }
            List<Role> roles = new List<Role>();
            List<int> roleId;
            try
            {
                roleId = u.RoleIds.ToList(',').Select(x => int.Parse(x)).ToList();
                roles = ServiceHelp.GetRoleService.GetAllList(x => roleId.Contains(x.Id)).ToList();
            }
            catch (Exception ex)
            {
                Task task= ServiceHelp.GetLogService.WriteErrorLogAsync(ex, Enums.LogLevel.Warning, "储存用户角色出现问题，格式转换失败。");
            }
            return roles;
        }

        /// <summary>
        /// 获取用户所有角色拥有的权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="errorMessage"></param>
        /// <returns>Null：失败；</returns>
        public List<string> GetRolePermissionsByUserId(int userId,out string errorMessage)
        {
            List<Role> list = GetRoleByUserId(userId, out errorMessage);
            var permissions = new List<string>();
            foreach (var item in list)
            {
                permissions= permissions.Union(item.Permissions.ToList(',')).ToList();
            }
            return permissions;
        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="role"></param>
        /// <param name="errorMessage"></param>
        /// <returns>Null：失败；</returns>
        public Role Add(Role role,out string errorMessage)
        {
            if (!VerifyData(role,out errorMessage))
            {
                return null;
            }
            var rRole= _service.Add(role);
            if (rRole == null)
            {
                errorMessage = "创建失败。";
                return null;
            }
            //记录操作日志
            Task task = ServiceHelp.GetLogService.WriteEventLogCreateAsync(typeof(Role), rRole.Id, rRole.ToJsonString());
            return rRole;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="role"></param>
        /// <param name="errorMessage"></param>
        /// <returns>Null：失败；</returns>
        public Role Edit(Role role,out string errorMessage)
        {
            if(!VerifyData(role,out errorMessage))
            {
                return null;
            }
            if (_service.GetById(role.Id)==null)
            {
                errorMessage = "未查询到数据！";
                return null;
            }
            var rRole = _service.Edit(role);
            if (rRole == null)
            {
                errorMessage = "修改失败。";
                return null;
            }
            //记录操作日志
            Task task = ServiceHelp.GetLogService.WriteEventLogEditAsync(typeof(Role), rRole.Id, rRole.ToJsonString());
            return rRole;
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="errorMessage"></param>
        /// <returns>True：成功；</returns>
        public bool Delete(int Id,out string errorMessage)
        {
            errorMessage = string.Empty;
            //判断有没有用户是这个角色
            if (ServiceHelp.GetUserService.GetAllList(x => (x.RoleIds + ",").Contains(Id.ToString() + ",")).Count > 0)
            {
                errorMessage = "有用户属于此角色，无法删除！";
                return false;
            }
            var rb= _service.DeleteById(Id);
            if (rb)
            {
                //删除成功，记录日志
                Task task = ServiceHelp.GetLogService.WriteEventLogDeleteAsync(typeof(Role), Id);
            }
            return rb;
        }

        /// <summary>
        /// 检测数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns>True：校验通过；</returns>
        private bool VerifyData(Role data,out string errorMessage)
        {
            errorMessage = string.Empty;
            if (data == null)
            {
                errorMessage = "请检查是否传入数据。";
                return false;
            }

            if (data.Id == 1)
            {
                errorMessage = "系统设置，无法修改！";
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
