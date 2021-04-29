using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using UserManage.Authorization;
using UserManage.Authorization.Roles;
using UserManage.Authorization.Users;
using UserManage.Roles.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using UserManage.Users.Dto;

namespace UserManage.Users
{   
    /// <summary>
    /// 用户角色服务
    /// </summary>
    [AbpAuthorize]
    public class UserRoleService : UserManageAppServiceBase, IUserRoleService
    {
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;

        public UserRoleService(RoleManager roleManager, UserManager userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        /// <summary>
        /// 根据角色与地区获取用户
        /// </summary>
        /// <param name="RoleName">角色名称</param>
        /// <param name="SelectDistrict">用户表地区字段</param>
        /// <returns></returns>
        public async Task<List<UserDto>> GetUserListByDistrictRoleAsync(string RoleName, string SelectDistrict)
        {
            var role = await _roleManager.GetRoleByNameAsync(RoleName);
            var users = await _userManager.GetUsersInRoleAsync(role.NormalizedName);
            var result = users.Where(x => x.SelectDistrict.Contains(SelectDistrict)).ToList();
            return ObjectMapper.Map<List<UserDto>>(result);
        }
    }
}
