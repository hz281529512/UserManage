using System;
using System.Collections.Generic;
using System.Text;
using UserManage.AbpExternalCore;
using UserManage.Users.Dto;

namespace UserManage.SynchronizeCore.Dtos
{
    public class GetWxUserDto : AbpWeChatUser
    {
        /// <summary>
        /// abp 用户
        /// </summary>
        public UserDto AbpUser { get; set; }

        /// <summary>
        /// baseuser 用户
        /// </summary>
        public BaseUserEmpDto BaseUserEmp { get; set; }

        /// <summary>
        /// baseuser 角色
        /// </summary>
        public List<BaseUserRoleDto> BaseUserRole { get; set; }

        /// <summary>
        /// baseuser 部门
        /// </summary>
        public List<BaseUserDeptDto> BaseUserOrg { get; set; }
    }
}
