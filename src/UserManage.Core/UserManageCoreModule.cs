using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Timing;
using Abp.Zero;
using Abp.Zero.Configuration;
using UserManage.Authorization.Roles;
using UserManage.Authorization.Users;
using UserManage.Configuration;
using UserManage.Localization;
using UserManage.MultiTenancy;
using UserManage.Timing;

namespace UserManage
{
    [DependsOn(typeof(AbpZeroCoreModule))]
    public class UserManageCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            // Declare entity types
            Configuration.Modules.Zero().EntityTypes.Tenant = typeof(Tenant);
            Configuration.Modules.Zero().EntityTypes.Role = typeof(Role);
            Configuration.Modules.Zero().EntityTypes.User = typeof(User);

            UserManageLocalizationConfigurer.Configure(Configuration.Localization);

            // Enable this line to create a multi-tenant application.
            Configuration.MultiTenancy.IsEnabled = UserManageConsts.MultiTenancyEnabled;

            // Configure roles
            AppRoleConfig.Configure(Configuration.Modules.Zero().RoleManagement);

            Configuration.Settings.Providers.Add<AppSettingProvider>();

            //Wechat Access Token
            Configuration.Caching.Configure(UserManageConsts.Abp_Wechat_Access_Token_Cache, cache => {
                cache.DefaultSlidingExpireTime = System.TimeSpan.FromHours(2);
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(UserManageCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<AppTimes>().StartupTime = Clock.Now;
        }
    }
}
