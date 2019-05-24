using System;
using System.Collections.Generic;
using System.Text;
using UserManage.AbpExternalCore.Model;

namespace UserManage.QYEmail
{
    internal class QYMailAccessToken : AbpWechatResult
    {
        public string access_token { get; set; }

        public int? expires_in { get; set; }
    }
}
