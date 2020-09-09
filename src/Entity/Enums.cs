using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entity
{
    public class Enums
    {
        /// <summary>
        /// 性别
        /// </summary>
        public enum Sex
        {
            /// <summary>
            /// 未知
            /// </summary>
            [Display(Name = "未知")]
            other,
            /// <summary>
            /// 男人
            /// </summary>
            [Display(Name ="男")]
            man,
            /// <summary>
            /// 女人
            /// </summary>
            [Display(Name = "女")]
            woman
        }

        /// <summary>
        /// 事件类型
        /// </summary>
        public enum EventType
        {
            /// <summary>
            /// 查看
            /// </summary>
            [Display(Name = "查看")]
            Select,
            /// <summary>
            /// 新增
            /// </summary>
            [Display(Name = "新增")]
            Insert,
            /// <summary>
            /// 修改
            /// </summary>
            [Display(Name = "修改")]
            Update,
            /// <summary>
            /// 删除
            /// </summary>
            [Display(Name = "删除")]
            Delete,
            /// <summary>
            /// 物理删除
            /// </summary>
            [Display(Name = "物理删除")]
            Delete_,
            /// <summary>
            /// 登录
            /// </summary>
            [Display(Name = "登录")]
            Login,
            /// <summary>
            /// 登出
            /// </summary>
            [Display(Name = "登出")]
            LogOut,
            /// <summary>
            /// 上传
            /// </summary>
            [Display(Name = "上传")]
            UpLoad,
            /// <summary>
            /// 下载
            /// </summary>
            [Display(Name = "下载")]
            DownLoad,
            /// <summary>
            /// 其它
            /// </summary>
            [Display(Name = "其它")]
            Other
        }

        /// <summary>
        /// 日志等级
        /// </summary>
        public enum LogLevel
        {
            /// <summary>
            /// 一般信息
            /// </summary>
            [Display(Name = "一般信息")]
            Info,
            /// <summary>
            /// 错误日志
            /// </summary>
            [Display(Name = "错误日志")]
            Error,
            /// <summary>
            /// 警告日志
            /// </summary>
            [Display(Name = "警告日志")]
            Warning
        }

        /// <summary>
        /// 项目类型
        /// </summary>
        public enum ProjectType
        {
            /// <summary>
            /// 微信公众号
            /// </summary>
            [Display(Name = "微信公众号")]
            WeChatMP,
            /// <summary>
            /// 微信小程序
            /// </summary>
            [Display(Name = "微信小程序")]
            WeChatApplet,
            /// <summary>
            /// 管理后台
            /// </summary>
            [Display(Name = "管理后台")]
            ManagementAdmin
        }

    }
}
