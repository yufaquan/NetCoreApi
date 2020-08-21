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


        /// <summary>
        /// 清空数据
        /// </summary>
        public static void Clear()
        {
            UserToken = null;
            UserId = null;
            UserJson = null;
            //userToken = null;
            //user = null;
        }

    }
}
