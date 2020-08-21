using Common;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Mangement
{
   public class RoleBussiness
    {
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
        /// <returns></returns>
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
    }
}
