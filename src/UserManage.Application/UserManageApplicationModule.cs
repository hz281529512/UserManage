using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using UserManage.Authorization;
using UserManage.AbpCompanyCore.Mapper;
using UserManage.AbpDataDictCore.Mapper;
using UserManage.BaseEntityCore.Mapper;
using UserManage.SynchronizeCore.Mapper;
using UserManage.ReceiveSyncCore.Mapper;

namespace UserManage
{
    [DependsOn(
        typeof(UserManageCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class UserManageApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<UserManageAuthorizationProvider>();

            //AutoMapper
            Configuration.Modules.AbpAutoMapper().Configurators.Add(AbpCompanyMapper.CreateMappings);
            Configuration.Modules.AbpAutoMapper().Configurators.Add(AbpDataDictMapper.CreateMappings);
            Configuration.Modules.AbpAutoMapper().Configurators.Add(BaseUserEmpRoleMapper.CreateMappings);
            Configuration.Modules.AbpAutoMapper().Configurators.Add(BaseUserOrgMapper.CreateMappings);
            Configuration.Modules.AbpAutoMapper().Configurators.Add(SynchronizeMapper.CreateMappings);
            Configuration.Modules.AbpAutoMapper().Configurators.Add(ReceiveSyncMapper.CreateMappings);

        }

        public override void Initialize()
        {
            var thisAssembly = typeof(UserManageApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddProfiles(thisAssembly)
            );
        }
    }
}
