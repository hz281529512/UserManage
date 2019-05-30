using System;
using System.Collections.Generic;
using System.Text;
using UserManage.AbpExternalCore.Model;

namespace UserManage.ThirdPartyConfigCore
{
    internal class SuiteAccessToken : AbpWechatResult
    {
        public string suite_access_token { get; set; }

        public int? expires_in { get; set; }
    }
}
