using System;

namespace CommonLib.Data
{
    public class Account
    {
        public uint id { get; set; }
        public string username { get; set; }

        public string password { get; set; }

        public bool is_online { get; set; }

        public DateTime? last_login_time { get; set; }

        public string last_login_ip { get; set; }

        public bool is_banned { get; set; }

    }
}
