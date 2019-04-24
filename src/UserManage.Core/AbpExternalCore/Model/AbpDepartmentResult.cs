using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.AbpExternalCore.Model
{
    /// <summary>
    /// 企业微信接口部门结果
    /// </summary>
    internal class AbpDepartmentResult : AbpWechatResult
    {
        /// <summary>
        /// 部门列表
        /// </summary>
        public ICollection<AbpWeChatDepartment> department { get; set; }
    }
}
