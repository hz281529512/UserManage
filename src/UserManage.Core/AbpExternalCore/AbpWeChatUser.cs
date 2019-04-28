using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.AbpExternalCore
{
    public class AbpWeChatUser
    {
        public string userid { get; set; }
        public string name { get; set; }

        public string mobile { get; set; }

        public string gender { get; set; }

        public string email { get; set; }

        public string avatar { get; set; }

        public string alias { get; set; }

        public string position { get; set; }

        public string qr_code { get; set; }

        public List<int> department { get; set; }
    }
}
