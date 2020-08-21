using System;
using System.Collections.Generic;
using System.Text;

namespace Authorization.Model
{
    /// <summary>
    /// 访问token
    /// </summary>
    public class VisitToken
    {
        /// <summary>
        /// 来自
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string ZZ { get; set; }
        /// <summary>
        /// 给谁
        /// </summary>
        public string To { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime STime { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public TimeSpan ETime { get; set; }
    }
}
