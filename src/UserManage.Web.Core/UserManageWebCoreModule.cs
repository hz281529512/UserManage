using System;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.SignalR;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Caching.Redis;
using Abp.Zero.Configuration;
using UserManage.Authentication.JwtBearer;
using UserManage.Configuration;
using UserManage.EntityFrameworkCore;

namespace UserManage
{
    [DependsOn(
         typeof(UserManageApplicationModule),
         typeof(UserManageEntityFrameworkModule),
         typeof(AbpAspNetCoreModule),
        typeof(AbpAspNetCoreSignalRModule),
        typeof(AbpRedisCacheModule)
     )]
    public class UserManageWebCoreModule : AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public UserManageWebCoreModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                UserManageConsts.ConnectionStringName
            );

            // Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            Configuration.Modules.AbpAspNetCore()
                 .CreateControllersForAppServices(
                     typeof(UserManageApplicationModule).GetAssembly()
                 );

            ConfigureTokenAuth();
            //Configuration.Caching.UseRedis();
        }

        private void ConfigureTokenAuth()
        {
            IocManager.Register<TokenAuthConfiguration>();
            var tokenAuthConfig = IocManager.Resolve<TokenAuthConfiguration>();
            if (bool.Parse(_appConfiguration["Authentication:JwtBearer:IsEnabled"]))
            {
                tokenAuthConfig.SecurityKey =
                    new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(_appConfiguration["Authentication:JwtBearer:SecurityKey"]));
                tokenAuthConfig.Issuer = _appConfiguration["Authentication:JwtBearer:Issuer"];
                tokenAuthConfig.Audience = _appConfiguration["Authentication:JwtBearer:Audience"];
                tokenAuthConfig.SigningCredentials =
                    new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256);
                tokenAuthConfig.Expiration = TimeSpan.FromDays(1);
            }
            else if (bool.Parse(_appConfiguration["Authentication:IdentityServer4:IsEnabled"]))
            {

                tokenAuthConfig.Authority = _appConfiguration["Authentication:IdentityServer4:Authority"];
                tokenAuthConfig.ClientId = _appConfiguration["Authentication:IdentityServer4:ClientId"];
                tokenAuthConfig.Secret = _appConfiguration["Authentication:IdentityServer4:Secret"];

            }

        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(UserManageWebCoreModule).GetAssembly());
        }
    }
}
