using Authorization;
using Common;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bussiness.Mangement
{
    public class UserBussiness
    {
        public static UserBussiness Init { get => new UserBussiness(); }

        /// <summary>
        /// 帐号密码登录
        /// </summary>
        /// <param name="name">电话/邮箱/名称</param>
        /// <param name="pwd">MD5一次后的密码</param>
        /// <param name="errorMessage">错误消息</param>
        /// <returns>UserToken</returns>
        public string LoginByPwd(string name,string pwd,out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(pwd))
            {
                errorMessage = "请输入帐号和密码！";
                return "";
            }
            pwd = Commons.GetMD5_32(pwd);
            User user= ServiceHelp.GetUserService.GetBy(x => x.Name == name || x.Mobile == name || x.Email == name);
            if (user==null)
            {
                errorMessage = "未查询到此帐号.";
                return "";
            }
            if (user.PassWordMD5==pwd)
            {
                //登录成功 获取token
                var usertoken= TokenHelp.WriteUserToken(user.Id);
                if (string.IsNullOrWhiteSpace(usertoken))
                {
                    errorMessage = "登录失败,请稍候重试。";
                    return "";
                }
                else
                {
                    //配置当前登录数据
                    Current.UserId = user.Id;
                    Current.UserToken = usertoken;
                    Current.UserJson = user.ToJsonString();
                    //记录登录日志
                    LogEvent log = new LogEvent();
                    log.EventType = Enums.EventType.Login;
                    log.Content = $"用户{user.Name}[{user.NickName}]登录了系统。";
                    log.UserId = user.Id;
                    log.UserName = user.Name;
                    log.WriteDate = DateTime.Now;
                    var task= ServiceHelp.GetLogService.WriteEventLogAsync(log);
                    return usertoken;
                }
            }
            else
            {
                errorMessage = "密码错误.";
                return "";
            }
        }

        /// <summary>
        /// 判断但前用户是否用于对应的权限
        /// </summary>
        /// <param name="user"></param>
        /// <param name="authorizeList">要判断的权限</param>
        /// <param name="errorMessage"></param>
        /// <returns>True：有权限；</returns>
        public bool IsHaveAuthorize(User user,List<string> authorizeList,out string errorMessage)
        {
            errorMessage = string.Empty;
            if (user==null)
            {
                user = Current.UserJson.JsonToModelOrDefault<User>();
            }
            if (user==null)
            {
                return false;
            }
            var ups = Commons.Split(user.Permissions, ',');
            //系统管理员角色和超级管理员拥有全部权限
            if (user.Id == 1 || user.RoleIds.ToList(',').Exists(x => x == "1"))
            {
                return true;
            }
            //获取用户角色的权限
            List<string> rlist = RoleBussiness.Init.GetRolePermissionsByUserId(user.Id, out errorMessage);

            var permissions = ups.Union(rlist);

            ////判断是否有权限
            foreach (var authorize in authorizeList)
            {
                if (permissions.Contains(authorize))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取用户拥有的权限
        /// </summary>
        /// <param name="user"></param>
        /// <param name="isAdmin">是否管理员</param>
        /// <returns></returns>
        public List<string> GetAllPermission(User user,out bool isAdmin,out string errorMessage)
        {
            errorMessage = string.Empty;
            isAdmin = false;
            if (user == null)
            {
                user = Current.UserJson.JsonToModelOrDefault<User>();
            }
            if (user == null)
            {
                return new List<string>();
            }
            var ups = Commons.Split(user.Permissions, ',');
            //系统管理员角色和超级管理员拥有全部权限
            if (user.Id == 1 || user.RoleIds.ToList(',').Exists(x => x == "1"))
            {
                isAdmin = true;
            }
            //获取用户角色的权限
            List<string> rlist = RoleBussiness.Init.GetRolePermissionsByUserId(user.Id, out errorMessage);

            var permissions = ups.Union(rlist);
            return permissions.ToList();
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public List<User> GetPageList(User user, int pageIndex, int pageSize, ref int pageCount)
        {
            System.Linq.Expressions.Expression<Func<User, bool>> where = null;
            if (!string.IsNullOrWhiteSpace(user.Name))
            {
                where= where.ExpressionAnd(x => x.Name.Contains(user.Name));
            }
            if (!string.IsNullOrWhiteSpace(user.NickName))
            {
                where = where.ExpressionAnd(x => x.NickName.Contains(user.NickName));
            }
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                where = where.ExpressionAnd(x => x.Email.Contains(user.Email));
            }
            if (!string.IsNullOrWhiteSpace(user.Mobile))
            {
                where = where.ExpressionAnd(x => x.Mobile.Contains(user.Mobile));
            }

            return ServiceHelp.GetUserService.GetPageList(where, pageIndex, pageSize, ref pageCount, x => x.CreatedAt, SqlSugar.OrderByType.Desc).ToList();
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="user"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public User Add(User user,out string errorMessage)
        {
            errorMessage = string.Empty;
            if (!VerifyData(user,out errorMessage))
            {
                return null;
            }
            return ServiceHelp.GetUserService.Add(user);
        }

        /// <summary>
        /// 核查数据
        /// </summary>
        /// <param name="user"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool VerifyData(User user,out string errorMessage)
        {
            errorMessage = string.Empty;
            if (user == null)
            {
                errorMessage = "未接收到数据。";
                return false;
            }
            if (string.IsNullOrWhiteSpace(user.Name))
            {
                errorMessage = "用户名不能为空。";
                return false;
            }
            else
            {
                var wlist = ServiceHelp.GetUserService.GetAllList(x => x.Name == user.Name);
                if (wlist != null && wlist.Count > 0)
                {
                    errorMessage = "此用户名已被注册。";
                    return false;
                }
            }
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                var wlist = ServiceHelp.GetUserService.GetAllList(x => x.Email == user.Email);
                if (wlist != null && wlist.Count > 0)
                {
                    errorMessage = "此邮箱已被注册。";
                    return false;
                }
            }
            if (!string.IsNullOrWhiteSpace(user.Mobile))
            {
                var wlist = ServiceHelp.GetUserService.GetAllList(x => x.Mobile == user.Mobile);
                if (wlist != null && wlist.Count > 0)
                {
                    errorMessage = "该手机号已被注册。";
                    return false;
                }
            }
            return true;
        }

    }
}
