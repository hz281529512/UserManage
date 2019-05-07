using System;
using System.Collections.Generic;
using System.Text;
using UserManage.AbpCompanyCore;

namespace UserManage.Users.Dto
{
    public class UserListDto : UserDto
    {
        public AbpCompany AbpCompany { get; set; }
    }
}
