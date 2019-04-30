using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.SynchronizeCore
{
    public class SyncUser
    {
        public string changetype { get; set; }

        public string userid { get; set; }
        public string name { get; set; }

        public string mobile { get; set; }

        public string gender { get; set; }

        public string email { get; set; }

        public string avatar { get; set; }

        public string alias { get; set; }

        public string position { get; set; }

        //public string qr_code { get; set; }

        public string department { get; set; }

        public int? status { get; set; }
    }
}
