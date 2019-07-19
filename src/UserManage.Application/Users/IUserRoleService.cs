using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UserManage.Users.Dto;

namespace UserManage.Users
{
    public interface IUserRoleService : IApplicationService
    {
        /// <summary>
        /// 根据角色与地区获取用户
        /// </summary>
        /// <param name="RoleName">角色名称</param>
        /// <param name="SelectDistrict">用户表地区字段</param>
        /// <returns></returns>
        Task<List<UserDto>> GetUserListByDistrictRoleAsync(string RoleName , string SelectDistrict);
    }
}
