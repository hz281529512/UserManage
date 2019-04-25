using System;
using System.Collections.Generic;
using System.Text;
using UserManage.Dtos;

namespace UserManage.Organizations.Dto
{
    public class FindOrganizationUnitUsersInput : PagedAndFilteredInputDto
    {
        public long OrganizationUnitId { get; set; }
    }
}
