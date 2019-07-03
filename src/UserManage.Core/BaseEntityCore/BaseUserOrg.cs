using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.BaseEntityCore
{
    public class BaseUserOrg : FullAuditedEntity<int>
    {
        public string WxId { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }
        
        public string OrgGuid { get; set; }

        public string OrgParentGuid { get; set; }

        public int? OrgOrderNo { get; set; }

    }
}
