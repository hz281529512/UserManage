using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UserManage.Organizations.Dto
{
    public class UsersToOrganizationUnitInput
    {
        public long[] UserIds { get; set; }

        [Range(1, long.MaxValue)]
        public long OrganizationUnitId { get; set; }
    }
}
