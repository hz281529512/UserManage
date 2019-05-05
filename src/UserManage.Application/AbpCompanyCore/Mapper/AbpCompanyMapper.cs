
using AutoMapper;
using UserManage.AbpCompanyCore;
using UserManage.AbpCompanyCore.Dtos;

namespace UserManage.AbpCompanyCore.Mapper
{

	/// <summary>
    /// 配置AbpCompany的AutoMapper
    /// </summary>
	internal static class AbpCompanyMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap <AbpCompany,AbpCompanyListDto>();
            configuration.CreateMap <AbpCompanyListDto,AbpCompany>();

            configuration.CreateMap <AbpCompanyEditDto,AbpCompany>();
            configuration.CreateMap <AbpCompany,AbpCompanyEditDto>();

        }
	}
}
