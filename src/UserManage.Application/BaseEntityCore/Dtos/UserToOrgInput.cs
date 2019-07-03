using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.BaseEntityCore.Dtos
{
    public class UserToOrgInput
    {
        /// <summary>
        /// Abp 用户ID
        /// </summary>
        public long? AbpUserId { get; set; }

        /// <summary>
        /// 组织GUID
        /// </summary>
        public string OrgGuid { get; set; }
    }
}
