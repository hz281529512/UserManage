using System.Collections.Generic;

namespace UserManage.Sessions.Dto
{
    public class GetCurrentLoginInformationsOutput
    {
        public ApplicationInfoDto Application { get; set; }

        public UserLoginInfoDto User { get; set; }

        public TenantLoginInfoDto Tenant { get; set; }
        public CompanyLoginInfo Company { get; set; }

        public List<OrgLoginInfo> ListOrg { get; set; }

        public string[] RoleNames { get; set; }

        public int[] RoleId { get; set; }
    }
}
