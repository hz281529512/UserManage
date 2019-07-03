using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.BaseEntityCore.Dtos
{
    public class OrgsToUserInput
    {
        public long? AbpUserId { get; set; }

        public string MasterOrgGuid { get; set; }

        public List<string> OrgGuids { get; set; }
    }
}
