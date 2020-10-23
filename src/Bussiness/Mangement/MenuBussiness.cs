using Authorization;
using Common;
using Entity;
using IService;
using Microsoft.AspNetCore.Authorization;
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
        public List<Menus> GetHavePermissionsList(bool isRoute=false)
        {
            string errorMessage;
            bool isAdmin;
            var sjklist = new List<Menus>();
            List<string> allP = UserBussiness.Init.GetAllPermission(null,out isAdmin,out errorMessage);
            if (isAdmin)
            {
                if (isRoute)
                    sjklist = ServiceHelp.GetMenuService.GetAllList(x => x.PId == 0).ToList();
                else
                    sjklist = ServiceHelp.GetMenuService.GetAllList(null).ToList();
            }
            else if(allP !=null && allP.Count>0)
            {
                if (isRoute)
                    sjklist = ServiceHelp.GetMenuService.GetAllList(x => x.PId==0 && allP.IsHaveSame(x.Permissions.ToList(','))).ToList();
                else
                    sjklist = ServiceHelp.GetMenuService.GetAllList(x => allP.IsHaveSame(x.Permissions.ToList(','))).ToList();
            }
            return sjklist;
        }

        /// <summary>
        /// 获取ElementUI所需的菜单列表
        /// </summary>
        /// <returns></returns>
        public List<Menus> GetAllElementList()
        {
            List<Menus> result = new List<Menus>();
            result = _service.GetAllList(x => x.PId == 0).ToList();
            //递归获取子菜单
            GetChildren(ref result);
            return result;
        }


        /// <summary>
        /// 获取有权限的ElementUI所需的菜单路由
        /// </summary>
        /// <returns></returns>
        public string GetHavePermissionsElementList()
        {
            List<Menus> result = GetHavePermissionsList(true);
            //递归获取子菜单
            GetChildren(ref result);
            result.Insert(0,new Menus()
            {
                Path = "/",
                Component = "Layout",
                Redirect = "/dashboard",
                Name = "Home",
                AlwaysShow = true,
                Children = new List<Menus>() { new Menus() { Path = "/dashboard", Name = "Dashboard", Component = "views/dashboard/index", Title = "首页", Icon = "dashboard" } }
            });
            return GetElementRoutes(result);
        }

        //递归获取子菜单
        private void GetChildren(ref List<Menus> list)
        {
            if (list==null || list.Count<=0)
            {
                return;
            }
            foreach (var item in list)
            {
                var clist= _service.GetAllList(x => x.PId == item.Id).ToList();
                GetChildren(ref clist);
                item.Children = clist;
            }
        }

        /// <summary>
        /// 转换成element 所需格式
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string GetElementRoutes(List<Menus> list)
        {
            StringBuilder str = new StringBuilder();
            str.Append("[");
            for (int i = 0; i < list.Count; i++)
            {
                str.Append("{");
                List<string> a = new List<string>();
                if (!string.IsNullOrWhiteSpace(list[i].Alias))
                {
                    a.Add($"\"alias\":\"{list[i].Alias}\"");
                }
                if (!string.IsNullOrWhiteSpace(list[i].Component))
                {
                    a.Add($"\"component\":\"{list[i].Component}\"");
                }
                if (list[i].AlwaysShow)
                {
                    a.Add($"\"alwaysShow\":true");
                }
                if (list[i].Children!=null && list[i].Children.Count>0)
                {
                    a.Add($"\"children\":{GetElementRoutes(list[i].Children)}");
                }
                if (list[i].Meta!=null && (!string.IsNullOrWhiteSpace(list[i].Meta.Title) || !string.IsNullOrWhiteSpace(list[i].Meta.Icon)))
                {
                    StringBuilder c = new StringBuilder();
                    c.Append("\"meta\":{");
                    List<string> aa = new List<string>();
                    if (!string.IsNullOrWhiteSpace(list[i].Meta.Title))
                    {
                        aa.Add($"\"title\":\"{list[i].Meta.Title}\"");
                    }
                    if (!string.IsNullOrWhiteSpace(list[i].Meta.Icon))
                    {
                        aa.Add($"\"icon\":\"{list[i].Meta.Icon}\"");
                    }
                    if (aa.Count>0)
                    {
                        c.Append(string.Join(",",aa));
                    }
                    c.Append("}");
                    a.Add(c.ToString());
                }
                if (!string.IsNullOrWhiteSpace(list[i].Name))
                {
                    a.Add($"\"name\":\"{list[i].Name}\"");
                }
                if (!string.IsNullOrWhiteSpace(list[i].Redirect))
                {
                    a.Add($"\"redirect\":\"{list[i].Redirect}\"");
                }
                if (!string.IsNullOrWhiteSpace(list[i].Path))
                {
                    a.Add($"\"path\":\"{list[i].Path}\"");
                }

                if (a.Count > 0)
                {
                    str.Append(string.Join(",", a));
                }
                str.Append("}");
                if (i<list.Count-1)
                {
                    str.Append(",");
                }
            }

            str.Append("]");

            return str.ToString();
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

    public class ElementRoutes
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 重定向
        /// </summary>
        public string Redirect { get; set; }

        /// <summary>
        /// 虚拟路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 菜单真实路径 
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// 子菜单
        /// </summary>
        public List<ElementRoutes> Children { get; set; }

        /// <summary>
        /// 是否一直显示
        /// </summary>
        public bool AlwaysShow { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public Meta Meta { get; set; }
    }
}
