using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.BaseEntityCore.Dtos
{
    public class OrgsMasterInput
    {
        /// <summary>
        /// abp 用户ID
        /// </summary>
        public long? AbpUserId { get; set; }

        /// <summary>
        /// F2BPM 主部门ID
        /// </summary>
        public string MasterOrgGuid { get; set; }
    }
}
