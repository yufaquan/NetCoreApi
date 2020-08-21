using System;
using System.Collections.Generic;
using System.Text;

namespace Authorization.Model
{
    public class UserToken
    {
        public int UserId { get; set; }
        public DateTime ITime { get; set; }
        public string Sign { get; set; }

    }
}
