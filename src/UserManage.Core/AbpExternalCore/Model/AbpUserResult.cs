using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.AbpExternalCore.Model
{
    internal class AbpUserResult : AbpWechatResult
    {
        /// <summary>
        /// 成员列表
        /// </summary>
        public List<AbpWeChatUser> userlist { get; set; }
    }
}
