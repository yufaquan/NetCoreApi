
using Entity;
using IService;
using Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bussiness
{
    /// <summary>
    /// 服务帮助类
    /// </summary>
    public class ServiceHelp
    {
        /// <summary>
        /// 获取用户服务
        /// </summary>
        public static IUserService GetUserService { get => new UserService(); }
        /// <summary>
        /// 获取日志服务
        /// </summary>
        public static LogService GetLogService { get => new LogService(); }

        /// <summary>
        /// 获取角色服务
        /// </summary>
        public static IRoleService GetRoleService { get => new RoleService(); }
        /// <summary>
        /// 获取菜单服务
        /// </summary>
        public static IMenuService GetMenuService { get => new MenuService(); }
        /// <summary>
        /// 获取用户扩展信息服务
        /// </summary>
        public static IUserInfoService GetUserInfoService { get => new UserInfoService(); }
    }
}
