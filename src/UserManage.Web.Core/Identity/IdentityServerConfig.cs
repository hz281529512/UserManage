using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IdentityServer4;
using IdentityServer4.Models;

namespace UserManage.Identity
{
    public static class IdentityServerConfig
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {

                new ApiResource("api1", "AbpApi"),
                new ApiResource("wechat","WechatApi")

            };
        }
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Phone()
            };
        }
        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client
                {
                    ClientId = "wechat",
                    ClientSecrets =
                    {
                        new Secret("Secret".Sha256())
                    },
                    AllowedScopes = {
                        "api1",
                        IdentityServerConstants.StandardScopes.OpenId, //必须要添加，否则报forbidden错误
                        IdentityServerConstants.StandardScopes.Profile

                    },
                    AllowedGrantTypes ={ "custom" },
                    AccessTokenType = AccessTokenType.Jwt,
                    AccessTokenLifetime=3600*24*7,
                    AlwaysSendClientClaims = true
                },
                new Client
                {
                    ClientId = "api1",
                    ClientSecrets =
                    {
                        new Secret("Secret".Sha256())
                    },
                    AllowedScopes = {
                        "api1",
                        IdentityServerConstants.StandardScopes.OpenId, //必须要添加，否则报forbidden错误
                        IdentityServerConstants.StandardScopes.Profile

                    },
                    AllowedGrantTypes =GrantTypes.ResourceOwnerPassword,
                    AccessTokenType = AccessTokenType.Jwt,
                    AccessTokenLifetime=3600*24*7,
                    AlwaysSendClientClaims = true
                },

            };
        }
    }
}
