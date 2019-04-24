using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.AbpExternalCore.Model
{
    internal class AbpTagUserResult : AbpWechatResult
    {
        /// <summary>
        /// 标签名
        /// </summary>
        public string tagname { get; set; }

        /// <summary>
        /// 简单用户列表
        /// </summary>
        public ICollection<AbpWeChatUser> userlist { get; set; }
    }
}
