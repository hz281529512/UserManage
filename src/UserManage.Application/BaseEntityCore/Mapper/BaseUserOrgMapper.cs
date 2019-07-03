
using AutoMapper;
using UserManage.BaseEntityCore;
using UserManage.BaseEntityCore.Dtos;

namespace UserManage.BaseEntityCore.Mapper
{

    /// <summary>
    /// 配置BaseUserOrg的AutoMapper
    /// </summary>
    internal static class BaseUserOrgMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<BaseUserOrg, BaseUserOrgListDto>();
            configuration.CreateMap<BaseUserOrgListDto, BaseUserOrg>();

            configuration.CreateMap<BaseUserOrgEditDto, BaseUserOrg>();
            configuration.CreateMap<BaseUserOrg, BaseUserOrgEditDto>();

            configuration.CreateMap<BaseUserOrg, BaseUserOrgDto>();
            configuration.CreateMap<BaseUserEmpOrg, BaseUserEmpOrgDto>();
        }
    }
}
