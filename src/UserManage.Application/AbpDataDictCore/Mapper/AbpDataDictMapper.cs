
using AutoMapper;
using UserManage.AbpDataDictCore;
using UserManage.AbpDataDictCore.Dtos;

namespace UserManage.AbpDataDictCore.Mapper
{

	/// <summary>
    /// 配置AbpDataDict的AutoMapper
    /// </summary>
	internal static class AbpDataDictMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap <AbpDataDict,AbpDataDictListDto>();
            configuration.CreateMap <AbpDataDictListDto,AbpDataDict>();

            configuration.CreateMap <AbpDataDictEditDto,AbpDataDict>();
            configuration.CreateMap <AbpDataDict,AbpDataDictEditDto>();

        }
	}
}
