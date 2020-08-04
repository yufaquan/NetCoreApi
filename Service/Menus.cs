using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Tally_API.Base
{
    //public static class MenuId
    //{

    //}
    public class Menus
    {
        public Menus(string name,string url)
        {
            Name = name;
            Url = url;
            Hide = false;
        }
        //private string id;
        //public string Id { get => id; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool Hide { get; set; }

        private List<string> authorizes;
        public List<string> Authorizes { get => authorizes; }

        private List<Menus> children;
        public List<Menus> Children { get=>children;}

        public Menus SetAuthorize(Type type)
        {
            if (authorizes==null)
            {
                authorizes = new List<string>();
            }
            authorizes.Add(type.ToString());
            return this;
        }

        public Menus SetAuthorize(Type[] types)
        {
            if (authorizes == null)
            {
                authorizes = new List<string>();
            }
            authorizes.AddRange(Array.ConvertAll<Type,string>(types,s=>s.ToString()));
            return this;
        }

        public Menus SetChildren(Menus menus)
        {
            if (children == null)
            {
                children = new List<Menus>();
            }
            children.Add(menus);
            return this;
        }

        public Menus SetChildren(Menus[] menus)
        {
            if (children == null)
            {
                children = new List<Menus>();
            }
            children.AddRange(menus);
            return this;
        }

    }

    public static class MenuService
    {
        public static List<Menus> menuList = new List<Menus>();

        //public static HtmlString GetLayuiHtmlString(Uri currentUrl)
        //{
        //    StringBuilder html = new StringBuilder();
        //    foreach (var menu in menuList)
        //    {
        //        bool isP = false;
        //        if (menu.Authorizes!=null && menu.Authorizes.Count>0)
        //        {
        //            isP = UserManagement.Initialize.IsHavePermission(menu.Authorizes);
        //        }
        //        if (!isP|| menu.Hide || (string.IsNullOrWhiteSpace(menu.Url) && (menu.Children == null || menu.Children.Count()== 0)))
        //        {
        //            continue;
        //        }
        //        string currentli = string.Empty, currentdd = string.Empty;
        //        Menus currentMenu;
        //        if (IsCurrentUrl(menu,currentUrl.AbsoluteUri.ToLower(),out currentMenu))
        //        {
        //            currentli = " layui-nav-itemed ";
        //        }
        //        if (currentMenu != null)
        //        {
        //            currentdd = " class='layui-this' ";
        //        }
        //        html.Append($"<li class='layui-nav-item{currentli}'>");
        //        if (menu.Children!=null && menu.Children.Count()>0)
        //        {
        //            html.Append($"<a href='javascript:;'>{menu.Name}</a><dl class='layui-nav-child'>");
        //            foreach (var children in menu.Children)
        //            {
        //                bool iscP = false;
        //                if (children.Authorizes != null && children.Authorizes.Count > 0)
        //                {
        //                    iscP = UserManagement.Initialize.IsHavePermission(children.Authorizes);
        //                }
        //                if (!iscP || children.Hide || (string.IsNullOrWhiteSpace(children.Url) && (children.Children == null || children.Children.Count() == 0)))
        //                {
        //                    continue;
        //                }
        //                if (children==currentMenu)
        //                {
        //                    html.Append($"<dd{currentdd}><a href='{(string.IsNullOrWhiteSpace(children.Url) ? "javascript:;" : children.Url)}'>{children.Name}</a></dd>");
        //                }
        //                else
        //                {
        //                    html.Append($"<dd><a href='{(string.IsNullOrWhiteSpace(children.Url) ? "javascript:;" : children.Url)}'>{children.Name}</a></dd>");
        //                }
        //            }
        //            html.Append("</dl>");
        //        }
        //        else
        //        {
        //            html.Append($"<a href='{(string.IsNullOrWhiteSpace(menu.Url)? "javascript:;" : menu.Url)}'>{menu.Name}</a>");
        //        }
        //        html.Append("</li>");
        //    }
        //    return new HtmlString(html.ToString());
        //}

        private static bool IsCurrentUrl(Menus menus,string currentUrl,out Menus currentMenu)
        {
            currentMenu = null;
            if (!string.IsNullOrWhiteSpace(menus.Url) && (menus.Children==null || menus.Children.Count==0))
            {
                if (currentUrl.Contains(menus.Url.ToLower()))
                {
                    return true;
                }
            }
            if (menus.Children != null && menus.Children.Count > 0)
            {
                foreach (var item in menus.Children)
                {
                    if (IsCurrentUrl(item,currentUrl))
                    {
                        currentMenu = item;
                        return true;
                    }
                }
            }
            return false;
        }
        private static bool IsCurrentUrl(Menus menus, string currentUrl)
        {
            if (!string.IsNullOrWhiteSpace(menus.Url) && (menus.Children == null || menus.Children.Count == 0))
            {
                if (currentUrl.Contains(menus.Url.ToLower()))
                {
                    return true;
                }
            }
            if (menus.Children != null && menus.Children.Count > 0)
            {
                foreach (var item in menus.Children)
                {
                    if (IsCurrentUrl(item, currentUrl))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }

}