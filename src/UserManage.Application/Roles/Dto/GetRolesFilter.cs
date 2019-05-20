using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.Roles.Dto
{
    public class GetRolesFilter : PagedResultRequestDto
    {

        public string Where { get; set; }


    }
}
