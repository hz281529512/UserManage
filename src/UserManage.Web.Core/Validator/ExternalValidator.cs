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
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using UserManage.Authentication.External;
using UserManage.Authorization;
using UserManage.Authorization.Users;
using UserManage.Models.TokenAuth;

namespace UserManage.Validator
{
    public class ExternalValidator : IExtensionGrantValidator
    {
        public IAbpSession AbpSession { get; set; }
        private readonly IExternalAuthManager _externalAuthManager;
        private readonly LogInManager _logInManager;
        private readonly ITenantCache _tenantCache;
        private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;
        private readonly UserRegistrationManager _userRegistrationManager;

        public ExternalValidator(IExternalAuthManager externalAuthManager, LogInManager logInManager, ITenantCache tenantCache, AbpLoginResultTypeHelper abpLoginResultTypeHelper, UserRegistrationManager userRegistrationManager)
        {
            _externalAuthManager = externalAuthManager;
            _logInManager = logInManager;
            _tenantCache = tenantCache;
            _abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            _userRegistrationManager = userRegistrationManager;
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
                var externalUser = await GetExternalUserInfo(model);

                var loginResult = await _logInManager.LoginAsync(
                    new UserLoginInfo(model.AuthProvider, externalUser.ProviderKey, model.AuthProvider),
                    GetTenancyNameOrNull());

                switch (loginResult.Result)
                {
                    case AbpLoginResultType.Success:
                        {
                            context.Result = new GrantValidationResult(
                                subject: loginResult.User.UserName,
                                authenticationMethod: "passwrod",
                                claims: CreateJwtClaims(loginResult.Identity)
                            );
                            break;
                        }
                    case AbpLoginResultType.UnknownExternalLogin:
                        {

                            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "未绑定");
                            break;
                            //if (!string.IsNullOrEmpty(externalUser.EmailAddress) ||
                            //    !string.IsNullOrEmpty(externalUser.Name))
                            //{
                            //    var newUser = await RegisterExternalUserAsync(externalUser);
                            //    if (!newUser.IsActive)
                            //    {
                            //        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "未激活");
                            //        break;
                            //    }

                            //    // Try to login again with newly registered user!
                            //    loginResult = await _logInManager.LoginAsync(
                            //        new UserLoginInfo(model.AuthProvider, externalUser.ProviderKey, model.AuthProvider),
                            //        GetTenancyNameOrNull());
                            //    if (loginResult.Result != AbpLoginResultType.Success)
                            //    {
                            //        context.Result = new GrantValidationResult(
                            //            subject: loginResult.User.UserName,
                            //            authenticationMethod: "passwrod",
                            //            claims: CreateJwtClaims(loginResult.Identity)
                            //        );
                            //        break;
                            //    }

                            //    context.Result = new GrantValidationResult(
                            //        subject: loginResult.User.UserName,
                            //        authenticationMethod: "passwrod",
                            //        claims: CreateJwtClaims(loginResult.Identity)
                            //    );
                            //    break;
                            //}

                            //    throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                            //        loginResult.Result,
                            //        externalUser.ProviderKey,
                            //        GetTenancyNameOrNull()
                            //    );

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
            //if (userInfo.ProviderKey != model.ProviderKey)
            //{
            //    throw new UserFriendlyException(L("CouldNotValidateExternalUser"));
            //}
            return userInfo;

        }

        private static List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == JwtRegisteredClaimNames.Sub);

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            claims.AddRange(new[]
            {
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
