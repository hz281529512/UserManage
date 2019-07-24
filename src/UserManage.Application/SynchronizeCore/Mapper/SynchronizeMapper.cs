using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using UserManage.AbpExternalCore;
using UserManage.Authorization.Users;
using UserManage.BaseEntityCore;
using UserManage.SynchronizeCore.Dtos;
using UserManage.Users.Dto;

namespace UserManage.SynchronizeCore.Mapper
{
    /// <summary>
    /// 配置Synchronize的AutoMapper
    /// </summary>
    internal static class SynchronizeMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<AbpWeChatUser, GetWxUserDto>();
            configuration.CreateMap<User, UserDto>();
            configuration.CreateMap<BaseUserEmp, BaseUserEmpDto>();
            configuration.CreateMap<BaseUserRole, BaseUserRoleDto>();
            configuration.CreateMap<BaseUserOrg, BaseUserDeptDto>();
        }
    }
}
