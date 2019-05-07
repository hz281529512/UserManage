using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.Users.Dto
{
    public class GetUsersInput : PagedResultRequestDto
    {
        public string Where { get; set; }
    }
}
