using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.Users.Dto
{
    public class BatchRoleUsers
    {
        public int RoleId { get; set; }

        public List<long> Userids { get; set; }
    }
}
