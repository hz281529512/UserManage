using System;
using System.Collections.Generic;
using System.Text;
using Abp.AutoMapper;
using Abp.Organizations;

namespace UserManage.Sessions.Dto
{
    [AutoMapFrom(typeof(OrganizationUnit))]
    public class OrgLoginInfo
    {
        public string DisplayName { get; set; }
    }
}
