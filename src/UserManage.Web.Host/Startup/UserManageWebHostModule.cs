using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using UserManage.Configuration;

namespace UserManage.Web.Host.Startup
{
    [DependsOn(
       typeof(UserManageWebCoreModule))]
    public class UserManageWebHostModule: AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public UserManageWebHostModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(UserManageWebHostModule).GetAssembly());

        }
    }
}
