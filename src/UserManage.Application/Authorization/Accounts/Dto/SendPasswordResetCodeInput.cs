using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.Authorization.Accounts.Dto
{
    public class SendPasswordResetCodeInput
    {

        public string UserName { get; set; }
        public string PhoneOrEmail { get; set; }

        public string ResetCode { get; set; }
    }
}
