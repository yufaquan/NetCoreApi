using IService;
using Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreAPI
{
    /// <summary>
    /// 服务帮助类
    /// </summary>
    public class ServiceHelp
    {
        public static IUserService GetUserService { get => new UserService(); }
    }
}
