using Authorization;
using Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreAPI
{
    public class AuthorizationService
    {
        /// <summary>
        /// 注册权限
        /// </summary>
        public static void LoadAuthorize()
        {
            AuthorizeData.Add(typeof(Read<User>), typeof(User));
            AuthorizeData.Add(typeof(Update<User>), typeof(User));
            AuthorizeData.Add(typeof(Create<User>), typeof(User));
            AuthorizeData.Add(typeof(Delete<User>), typeof(User));

            AuthorizeData.Add(typeof(Read<Menus>), typeof(Menus));
            AuthorizeData.Add(typeof(Update<Menus>), typeof(Menus));
            AuthorizeData.Add(typeof(Create<Menus>), typeof(Menus));
            AuthorizeData.Add(typeof(Delete<Menus>), typeof(Menus));


        }
    }
}
