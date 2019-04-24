using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.AbpExternalCore
{
    /// <summary>
    /// 企业微信 - 标签
    /// </summary>
    public class AbpWeChatTag
    {
        /// <summary>
        /// 标签id
        /// </summary>
        public int? tagid { get; set; }

        /// <summary>
        /// 标签名
        /// </summary>
        public string tagname { get; set; }
    }
}
