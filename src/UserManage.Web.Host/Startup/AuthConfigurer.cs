using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Abp.Runtime.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Logging;
using UserManage.Authentication.External;
using UserManage.Authentication.External.Wechat;
using UserManage.EntityFrameworkCore;

namespace UserManage.Web.Host.Startup
{
    public static class AuthConfigurer
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            if (bool.Parse(configuration["Authentication:JwtBearer:IsEnabled"]))
            {
                services.AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = "JwtBearer";
                    options.DefaultChallengeScheme = "JwtBearer";
                }).AddJwtBearer("JwtBearer", options =>
                {
                    options.Audience = configuration["Authentication:JwtBearer:Audience"];

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // The signing key must match!
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Authentication:JwtBearer:SecurityKey"])),

                        // Validate the JWT Issuer (iss) claim
                        ValidateIssuer = true,
                        ValidIssuer = configuration["Authentication:JwtBearer:Issuer"],

                        // Validate the JWT Audience (aud) claim
                        ValidateAudience = true,
                        ValidAudience = configuration["Authentication:JwtBearer:Audience"],

                        // Validate the token expiry
                        ValidateLifetime = true,

                        // If you want to allow a certain amount of clock drift, set that here
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = QueryStringTokenResolver
                    };
                });
            }
            else if (bool.Parse(configuration["Authentication:IdentityServer4:IsEnabled"]))
            {
                IdentityModelEventSource.ShowPII = true;
                services.AddAuthentication()
                    .AddIdentityServerAuthentication(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.ApiName = configuration["Authentication:IdentityServer4:ApiName"];
                        options.Authority = configuration["Authentication:IdentityServer4:Authority"];

                        options.RequireHttpsMetadata = false;
                    });


            }

            //services.AddMvc(
            //    options=> options.Filters.Add(new AutoValidateAntiforgeryTokenAuthorization)
            //    )

        }

        public static void ExternalAuth(IApplicationBuilder app, IConfiguration configuration)
        {
            var externalAuthConfiguration = app.ApplicationServices.GetRequiredService<ExternalAuthConfiguration>();
            
            if (bool.Parse(configuration["Authentication:Wechat:IsEnabled"]))
            {
                externalAuthConfiguration.Providers.Add
                (
                    new ExternalLoginProviderInfo(
                        WechatAuthProviderApi.Name,
                        configuration["Authentication:Wechat:AppId"],
                        configuration["Authentication:Wechat:Secret"],
                        typeof(WechatAuthProviderApi)
                    )
                );
            }

          
        }
        /* This method is needed to authorize SignalR javascript client.
         * SignalR can not send authorization header. So, we are getting it from query string as an encrypted text. */
        private static Task QueryStringTokenResolver(MessageReceivedContext context)
        {
            if (!context.HttpContext.Request.Path.HasValue ||
                !context.HttpContext.Request.Path.Value.StartsWith("/signalr"))
            {
                // We are just looking for signalr clients
                return Task.CompletedTask;
            }

            var qsAuthToken = context.HttpContext.Request.Query["enc_auth_token"].FirstOrDefault();
            if (qsAuthToken == null)
            {
                // Cookie value does not matches to querystring value
                return Task.CompletedTask;
            }

            // Set auth token from cookie
            context.Token = SimpleStringCipher.Instance.Decrypt(qsAuthToken, AppConsts.DefaultPassPhrase);
            return Task.CompletedTask;
        }


    }
}
