using Bussiness;
using Common;
using Entity;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeChatRelated.MP
{
    public class UserService
    {
        /// <summary>
        /// 根据微信的用户信息创建或修改系统用户
        /// </summary>
        /// <param name="userInfo">微信的用户信息</param>
        /// <returns></returns>
        public User InsertOrUpdateByWXMPUserInfo(OAuthUserInfo userInfo)
        {
            var userInfoService= ServiceHelp.GetUserInfoService;
            var uService = ServiceHelp.GetUserService;
            //判断是否有系统帐号
            var uinfo = userInfoService.GetAllList(x => x.OpenId == userInfo.openid && x.ProjectType == Entity.Enums.ProjectType.WeChatMP).FirstOrDefault();
            User result = null;
            if (uinfo == null)
            {
                //创建
                User u = new User();
                u.Area = $"{userInfo.country}-{userInfo.province}-{userInfo.city}";
                u.HeadImgUrl = userInfo.headimgurl;
                do
                {
                    //u.Name = Common.Commons.RndCode(6, "temporary_", "");
                    u.Name = Commons.RndCode(10, "auto_", "");
                } while (uService.GetAllList(x => x.Name == u.Name).Count > 0);//判断数据库中是否存在
                u.NickName = userInfo.nickname;
                u.Sex = (Enums.Sex)userInfo.sex;
                u.UnionId = userInfo.unionid;
                //进行事物操作
                try
                {
                    uService.BeginTran();

                    result = uService.Add(u);
                    if (result == null)
                    {
                        return result;
                    }
                    UserInfo ui = new UserInfo();
                    ui.OpenId = userInfo.openid;
                    ui.ProjectType = Enums.ProjectType.WeChatMP;
                    ui.UserId = result.Id;
                    var rui= userInfoService.Add(ui);
                    if (rui==null)
                    {
                        throw new Exception("error");
                    }

                    uService.CommitTran();
                }
                catch (Exception)
                {
                    uService.RollbackTran();
                    //输出0 ,没错虽然不在同一个dal里面但是 studentDal 成功回滚了 schoolDal的插入操作
                }
                return result;
            }
            else
            {
                //修改昵称和头像
                var u = uService.GetById(uinfo.UserId);
                if (u!=null)
                {
                    u.HeadImgUrl = userInfo.headimgurl;
                    u.NickName = userInfo.nickname;
                    u.Sex = (Enums.Sex)userInfo.sex;
                    result = uService.Edit(u);
                }
                return result;
            }
        }
    }
}
