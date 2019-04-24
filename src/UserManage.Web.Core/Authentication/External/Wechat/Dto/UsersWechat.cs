using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.Authentication.External.Wechat.Dto
{
    public class UsersWechat
    {
        public string userid { get; set; }
        public string name { get; set; }

        public string mobile { get; set; }

        public string gender { get; set; }

        public string email { get; set; }

        public string avatar { get; set; }

        public string qr_code { get; set; }

    }
}
