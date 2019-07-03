using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.BaseEntityCore.Dtos
{
    public class BaseUserOrgDto : BaseUserOrgListDto
    {

        public int? MemberCount { get; set; }

        public bool? IsMaster { get; set; }
    }
}
