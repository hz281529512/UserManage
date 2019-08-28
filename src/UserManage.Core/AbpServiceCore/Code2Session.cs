using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.AbpServiceCore
{
    public class Code2Session
    {
        public string corpid { get; set; }
        public string session_key { get; set; }

        public string deviceid { get; set; }

        public string userid { get; set; }

        public string errcode { get; set; }

        public string errMsg { get; set; }
    }
}
