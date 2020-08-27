using Authorization;
using Common;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bussiness.Mangement
{
    public class MenusBussiness
    {
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
    }
}
