using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Abp.UI;
using IdentityModel.Client;
using IdentityServer4.Validation;
using UserManage.Authentication.External;
using UserManage.Authentication.JwtBearer;
using UserManage.Authorization;
using UserManage.Authorization.Users;
using UserManage.Models.TokenAuth;
using UserManage.MultiTenancy;
using UserManage.Validator;
using UserManage.Authentication.External.Wechat;
using UserManage.AbpServiceCore.DomainService;
using UserManage.AbpServiceCore;

namespace UserManage.Controllers
{   
    /// <summary>
    /// token授权
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class TokenAuthController : UserManageControllerBase
    {
        private readonly LogInManager _logInManager;
        private readonly ITenantCache _tenantCache;
        private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;
        private readonly TokenAuthConfiguration _configuration;
        private readonly IExternalAuthConfiguration _externalAuthConfiguration;
        private readonly IExternalAuthManager _externalAuthManager;
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly IServiceAuthManager _serviceAuthManager;


        public TokenAuthController(
            LogInManager logInManager,
            ITenantCache tenantCache,
            AbpLoginResultTypeHelper abpLoginResultTypeHelper,
            TokenAuthConfiguration configuration,
            IExternalAuthConfiguration externalAuthConfiguration,
            IExternalAuthManager externalAuthManager,
            UserRegistrationManager userRegistrationManager,
            IServiceAuthManager serviceAuthManager)
        {
            _logInManager = logInManager;
            _tenantCache = tenantCache;
            _abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            _configuration = configuration;
            _externalAuthConfiguration = externalAuthConfiguration;
            _externalAuthManager = externalAuthManager;
            _userRegistrationManager = userRegistrationManager;
            _serviceAuthManager = serviceAuthManager;
        }

        /// <summary>
        /// 用户验证
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AuthenticateResultModel> Authenticate([FromBody] AuthenticateModel model)
        {
            //var loginResult = await GetLoginResultAsync(
            //    model.UserNameOrEmailAddress,
            //    model.Password,
            // GetTenancyNameOrNull()
            //);


            //var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));

            //return new AuthenticateResultModel
            //{
            //    AccessToken = accessToken,
            //    EncryptedAccessToken = GetEncrpyedAccessToken(accessToken),
            //    ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds,
            //    UserId = loginResult.User.Id
            //};

            var httpHandler = new HttpClientHandler();
            httpHandler.CookieContainer.Add(
                new Uri(_configuration.Authority),
                new Cookie("Abp.TenantId", AbpSession.TenantId?.ToString())
            ); //Set TenantId
            var tokenClient = new TokenClient(
                $"{_configuration.Authority}/connect/token",
                "api1",
                _configuration.Secret,
                httpHandler);
            var tokenResponse = tokenClient
                .RequestResourceOwnerPasswordAsync(model.UserNameOrEmailAddress, model.Password)
                .ConfigureAwait(false)
                .GetAwaiter().GetResult();

            if (tokenResponse.IsError)
            {
                throw new UserFriendlyException(tokenResponse.Error);
            }
            var data = new AuthenticateResultModel
            {
                AccessToken = tokenResponse.AccessToken,
                EncryptedAccessToken = GetEncrpyedAccessToken(tokenResponse.AccessToken),
                ExpireInSeconds = tokenResponse.ExpiresIn,

            };
            return data;
        }
        /// <summary>
        /// 获取外部认证提供程序
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<ExternalLoginProviderInfoModel> GetExternalAuthenticationProviders()
        {
            return ObjectMapper.Map<List<ExternalLoginProviderInfoModel>>(_externalAuthConfiguration.Providers);
        }
        /// <summary>
        /// 外部验证
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ExternalAuthenticateResultModel> ExternalAuthenticate([FromBody] ExternalAuthenticateModel model)
        {
            var externalUser = await GetUserInfo(model);

            var tenan_name = GetTenancyNameOrNull() ?? "Cailian";

            var loginResult = await _logInManager.LoginAsync(new UserLoginInfo(externalUser.Provider, externalUser.ProviderKey, externalUser.Provider), tenan_name);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    {
                        var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));
                        return new ExternalAuthenticateResultModel
                        {
                            AccessToken = accessToken,
                            EncryptedAccessToken = GetEncrpyedAccessToken(accessToken),
                            ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
                        };
                    }
                case AbpLoginResultType.UnknownExternalLogin:
                    {
                        var newUser = await RegisterExternalUserAsync(externalUser);
                        if (!newUser.IsActive)
                        {
                            return new ExternalAuthenticateResultModel
                            {
                                WaitingForActivation = true
                            };
                        }

                        // Try to login again with newly registered user!
                        loginResult = await _logInManager.LoginAsync(new UserLoginInfo(model.AuthProvider, externalUser.ProviderKey, model.AuthProvider), GetTenancyNameOrNull());
                        if (loginResult.Result != AbpLoginResultType.Success)
                        {
                            throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                                loginResult.Result,
                                externalUser.ProviderKey,
                                GetTenancyNameOrNull()
                            );
                        }

                        return new ExternalAuthenticateResultModel
                        {
                            AccessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity)),
                            ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
                        };
                    }
                default:
                    {
                        throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                            loginResult.Result,
                            externalUser.ProviderKey,
                            GetTenancyNameOrNull()
                        );
                    }
            }
        }

        private async Task<User> RegisterExternalUserAsync(AuthUserInfo externalUser)
        {
            var user = await _userRegistrationManager.RegisterAsync(
                externalUser.Name,
                externalUser.PhoneNumber,
                //externalUser.Surname,
                externalUser.EmailAddress,
                externalUser.EmailAddress,
                //Authorization.Users.User.CreateRandomPassword(),
                "000000",
                true
            );

            user.Logins = new List<UserLogin>
            {
                new UserLogin
                {
                    LoginProvider = externalUser.Provider,
                    ProviderKey = externalUser.ProviderKey,
                    TenantId = user.TenantId
                }
            };

            //await CurrentUnitOfWork.SaveChangesAsync();

            return user;
        }

        private async Task<ExternalAuthUserInfo> GetExternalUserInfo(ExternalAuthenticateModel model)
        {
            var userInfo = await _externalAuthManager.GetUserInfo(model.AuthProvider, model.ProviderAccessCode);


            //if (userInfo.ProviderKey != model.ProviderKey)
            //{
            //    throw new UserFriendlyException(L("CouldNotValidateExternalUser"));
            //}

            return userInfo;
        }

        private async Task<AuthUserInfo> GetUserInfo(ExternalAuthenticateModel model)
        {
            var userInfo = await _serviceAuthManager.GetUserInfo(model.AuthProvider, model.ProviderAccessCode);

            return userInfo;
        }

        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
        }

        private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, tenancyName);
            }
        }

        private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            var now = DateTime.UtcNow;

            var t_expires = now.Add(expiration ?? _configuration.Expiration);
            if (t_expires <= now) t_expires = t_expires.AddDays(1);


            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                notBefore: now,
                expires: t_expires,
                signingCredentials: _configuration.SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private static List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == JwtRegisteredClaimNames.Sub);

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });

            return claims;
        }

        private string GetEncrpyedAccessToken(string accessToken)
        {
            return SimpleStringCipher.Instance.Encrypt(accessToken, AppConsts.DefaultPassPhrase);
        }
        /// <summary>
        /// 获取令牌
        /// </summary>
        /// <param name="local_name"></param>
        /// <param name="CropId"></param>
        /// <param name="Secret"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> TryGetToken(string local_name, string CropId, string Secret)
        {
            return await _serviceAuthManager.GetToken(local_name, CropId, Secret);
        }
    }
}
