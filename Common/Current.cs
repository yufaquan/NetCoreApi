namespace Common
{
    public class Current
    {
        /// <summary>
        /// 当前访问的用户token
        /// 在访问结束、抛出异常后销毁
        /// </summary>
        public static string CurrentUserToken { get; set; }
        public static int? CurrentUserId { get; set; }


        /// <summary>
        /// 清空数据
        /// </summary>
        public static void Clear()
        {
            CurrentUserToken = null;
            CurrentUserId = null;
            //userToken = null;
            //user = null;
        }

    }
}
