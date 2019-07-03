using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.BaseEntityCore
{

    public class BaseUserEmpRole : FullAuditedEntity<int>
    {
        public string BaseUniqueId { get; set; }

        public string AbpUserId { get; set; }

        public string EmpUserId { get; set; }
        
        public string EmpUserGuid { get; set; }

        public int BaseRoleId { get; set; }
    }
}
