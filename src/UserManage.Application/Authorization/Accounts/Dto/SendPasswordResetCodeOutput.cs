using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.Authorization.Accounts.Dto
{
    public class SendPasswordResetCodeOutput
    {
        public int Id { get; set; }

        public string UserName { get; set; }

    }
}
