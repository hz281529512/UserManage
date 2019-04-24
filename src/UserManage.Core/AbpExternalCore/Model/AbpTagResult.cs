using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.AbpExternalCore.Model
{
    /// <summary>
    /// 企业微信 标签列表结果
    /// </summary>
    internal class AbpTagResult : AbpWechatResult
    {
        /// <summary>
        /// 标签列表
        /// </summary>
        public ICollection<AbpWeChatTag> taglist { get; set; }

    }
}
