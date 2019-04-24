using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.AbpExternalCore.Model
{
    internal class AbpUserDepartmentResult : AbpWechatResult
    {
        /// <summary>
        /// 简单用户列表
        /// </summary>
        public ICollection<AbpWeChatUserDepartment> userlist { get; set; }
    }
}
