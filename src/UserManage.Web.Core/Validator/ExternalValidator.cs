using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
using AutoMapper;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using UserManage.AbpCompanyCore.DomainService;
using UserManage.AbpServiceCore;
using UserManage.AbpServiceCore.DomainService;
using UserManage.AbpUserRoleCore.DomainService;
using UserManage.Authentication.External;
using UserManage.Authorization;
using UserManage.Authorization.Users;
using UserManage.Models.TokenAuth;
using UserManage.MultiTenancy;
using UserManage.Sessions.Dto;

namespace UserManage.Validator
{
    public class ExternalValidator : IExtensionGrantValidator
    {
        public IAbpSession AbpSession { get; set; }
        public UserManager UserManager { get; set; }
        public UserRoleManager UserRoleManager { get; set; }

        public AbpCompanyManager CompanyManager { get; set; }
        private readonly IExternalAuthManager _externalAuthManager;
        private readonly LogInManager _logInManager;
        private readonly ITenantCache _tenantCache;
        private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly IServiceAuthManager _serviceAuthManager;

        public ExternalValidator(IExternalAuthManager externalAuthManager, LogInManager logInManager, ITenantCache tenantCache, AbpLoginResultTypeHelper abpLoginResultTypeHelper, UserRegistrationManager userRegistrationManager,
            IServiceAuthManager serviceAuthManager)
        {
            _externalAuthManager = externalAuthManager;
            _logInManager = logInManager;
            _tenantCache = tenantCache;
            _abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            _userRegistrationManager = userRegistrationManager;

            _serviceAuthManager = serviceAuthManager;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {

            var code = context.Request.Raw.Get("code");
            var auth = context.Request.Raw.Get("auth");
            ExternalAuthenticateModel model = new ExternalAuthenticateModel()
            {
                AuthProvider = auth,
                ProviderAccessCode = code
            };
            var externalUser = await GetUserInfo(model);//await GetExternalUserInfo(model);

            //var loginResult = await _logInManager.LoginAsync(
            //    new UserLoginInfo(model.AuthProvider, externalUser.ProviderKey, model.AuthProvider),
            //    GetTenancyNameOrNull());
            var loginResult = await _logInManager.LoginAsync(
                new UserLoginInfo(externalUser.Provider, externalUser.ProviderKey, externalUser.Provider),
                GetTenancyNameOrNull());

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    {
                        context.Result = new GrantValidationResult(
                            subject: loginResult.Identity.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value,
                            authenticationMethod: "passwrod",
                            claims: await CreateJwtClaims(loginResult)
                        );
                        break;
                    }
                case AbpLoginResultType.UnknownExternalLogin:
                    {

                        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "未绑定");
                        break;


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

        public string GrantType
        {
            get { return "custom"; }
        }
        private async Task<ExternalAuthUserInfo> GetExternalUserInfo(ExternalAuthenticateModel model)
        {

            var userInfo = await _externalAuthManager.GetUserInfo(model.AuthProvider, model.ProviderAccessCode);
            //var userInfo = await _externalAuthManager.GetWechatUserInfo(model.AuthProvider, model.ProviderAccessCode);

            return userInfo;

        }

        private async Task<AuthUserInfo> GetUserInfo(ExternalAuthenticateModel model)
        {

            var userInfo = await _serviceAuthManager.GetUserInfo(model.AuthProvider, model.ProviderAccessCode);
            //var userInfo = await _externalAuthManager.GetWechatUserInfo(model.AuthProvider, model.ProviderAccessCode);

            return userInfo;

        }

        private async Task<List<Claim>> CreateJwtClaims(AbpLoginResult<Tenant, User> loginResult)
        {
            var claims = loginResult.Identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == JwtRegisteredClaimNames.Sub);
            string userModel = JsonConvert.SerializeObject(Mapper.Map<UserLoginInfoDto>(loginResult.User));
            var org = await UserManager.GetOrganizationUnitsAsync(loginResult.User);
            string orgModel = JsonConvert.SerializeObject(Mapper.Map<List<OrgLoginInfo>>(org));
            var company = await CompanyManager.FindByIdAsync(loginResult.User.CompanyId);
            string companyModel = JsonConvert.SerializeObject(Mapper.Map<CompanyLoginInfo>(company));
            var roles = await UserManager.GetRolesAsync(loginResult.User);
            var max_role_type = await UserRoleManager.MaxRoleType(roles);
            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            claims.AddRange(new[]
            {
                new Claim("UserModel",userModel),
                new Claim("OrgModel",orgModel),
                new Claim("CompanyModel",companyModel),
                new Claim("MaxRoleType",max_role_type.ToString()),
                new Claim(ClaimTypes.NameIdentifier, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });

            return claims;
        }


        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
        }

        private async Task<User> RegisterExternalUserAsync(ExternalAuthUserInfo externalUser)
        {
            var user = await _userRegistrationManager.RegisterAsync(
                externalUser.Name,
                externalUser.PhoneNumber,
                //externalUser.Surname,
                externalUser.EmailAddress,
                externalUser.EmailAddress,
                "000000",
                //Authorization.Users.User.CreateRandomPassword(),
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
    }
}
