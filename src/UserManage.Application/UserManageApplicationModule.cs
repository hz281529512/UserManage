using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using UserManage.Authorization;

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
