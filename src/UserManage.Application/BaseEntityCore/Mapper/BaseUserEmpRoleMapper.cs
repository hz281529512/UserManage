
using AutoMapper;
using UserManage.BaseEntityCore;
using UserManage.BaseEntityCore.Dtos;

namespace UserManage.BaseEntityCore.Mapper
{

	/// <summary>
    /// 配置BaseUserEmpRole的AutoMapper
    /// </summary>
	internal static class BaseUserEmpRoleMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap <BaseUserEmpRole,BaseUserEmpRoleListDto>();
            configuration.CreateMap <BaseUserEmpRoleListDto,BaseUserEmpRole>();

            configuration.CreateMap <BaseUserEmpRoleEditDto,BaseUserEmpRole>();
            configuration.CreateMap <BaseUserEmpRole,BaseUserEmpRoleEditDto>();

            configuration.CreateMap<BaseUserRole, BaseUserRoleListDto>();
            configuration.CreateMap<BaseUserRoleListDto, BaseUserRole>();

            configuration.CreateMap<BaseUserRoleEditDto, BaseUserRole>();
            configuration.CreateMap<BaseUserRole, BaseUserRoleEditDto>();
        }
	}
}
