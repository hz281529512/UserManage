using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.AbpServiceCore
{
    public class AuthUserInfo
    {
        public string ProviderKey { get; set; }

        public string Name { get; set; }

        public string EmailAddress { get; set; }

        // public string Surname { get; set; }

        public string Provider { get; set; }

        public string PhoneNumber { get; set; }
    }
}
