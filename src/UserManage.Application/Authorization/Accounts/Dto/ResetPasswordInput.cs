using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.Authorization.Accounts.Dto
{
    public class ResetPasswordInput
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string ResetCode { get; set; }
        public string Password { get; set; }
    }
}
