using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.Authorization.Accounts.Dto
{
    public class ResetPasswordOutput
    {
        public bool CanLogin { get; set; }

        public string UserName { get; set; }
    }
}
