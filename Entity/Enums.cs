using System;
using System.Collections.Generic;
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
            other,
            /// <summary>
            /// 男人
            /// </summary>
            man,
            /// <summary>
            /// 女人
            /// </summary>
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
            Select, 
            /// <summary>
            /// 新增
            /// </summary>
            Insert,
            /// <summary>
            /// 修改
            /// </summary>
            Update, 
            /// <summary>
            /// 删除
            /// </summary>
            Delete, 
            /// <summary>
            /// 物理删除
            /// </summary>
            Delete_,
            /// <summary>
            /// 登录
            /// </summary>
            Login,
            /// <summary>
            /// 登出
            /// </summary>
            LogOut,
            /// <summary>
            /// 上传
            /// </summary>
            UpLoad,
            /// <summary>
            /// 下载
            /// </summary>
            DownLoad,
            /// <summary>
            /// 其它
            /// </summary>
            Other
        }
    }
}
