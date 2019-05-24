using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.QYEmail
{
    public class QYMailUserInfocs
    {

        public string userid { get; set; }

        public string name { get; set; }

        public List<long> department { get; set; }

        public string position { get; set; }

        public string mobile { get; set; }

        public string tel { get; set; }

        public string extid { get; set; }

        public string gender { get; set; }

        public string password { get; set; }


    }

    public class QYMailUserInfocsForSeach : QYMailUserInfocs
    {
        public string errcode { get; set; }

        public string errmsg { get; set; }

        public int? enable { get; set; }
    }

    public class QYMailUserInfocsForUpdate : QYMailUserInfocs {

        public string access_token { get; set; }

        public int? enable { get; set; }
    }
}
