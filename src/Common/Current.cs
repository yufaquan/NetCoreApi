namespace Common
{
    public class Current
    {
        /// <summary>
        /// 当前访问的用户token
        /// 在访问结束、抛出异常后销毁
        /// </summary>
        public static string UserToken { get; set; }
        public static int? UserId { get; set; }

        public static string UserJson { get; set; }

        public static string VisitToKey { get; set; }
        /// <summary>
        /// 微信
        /// </summary>
        public static string WxOpenId { get; set; }
        public static string WxUnionId { get; set; }

        /// <summary>
        /// 清空数据
        /// </summary>
        public static void Clear()
        {
            UserToken = null;
            UserId = null;
            UserJson = null;
            VisitToKey = null;
            WxOpenId = null;
            WxUnionId = null;
        }

        /// <summary>
        /// 项目根目录
        /// </summary>
        public static string ServerPath { get; set; }
    }
}
