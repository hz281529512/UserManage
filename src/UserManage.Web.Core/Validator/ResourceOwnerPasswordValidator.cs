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
using Newtonsoft.Json;
using UserManage.AbpCompanyCore.DomainService;
using UserManage.AbpUserRoleCore.DomainService;
using UserManage.Authorization;
using UserManage.Authorization.Users;
using UserManage.MultiTenancy;
using UserManage.Sessions.Dto;

namespace UserManage.Validator
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public IAbpSession AbpSession { get; set; }
        public UserManager UserManager { get; set; }
        public AbpCompanyManager CompanyManager { get; set; }

        public UserRoleManager UserRoleManager { get; set; }

        private readonly LogInManager _logInManager;
        private readonly ITenantCache _tenantCache;
        private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;
        //private readonly TokenAuthConfiguration _configuration;


        public ResourceOwnerPasswordValidator(LogInManager logInManager,
            ITenantCache tenantCache,
            AbpLoginResultTypeHelper abpLoginResultTypeHelper)
        {
            _logInManager = logInManager;
            _tenantCache = tenantCache;
            _abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            //_configuration = configuration;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                var loginResult = await GetLoginResultAsync(
                         context.UserName,
                         context.Password,
                         GetTenancyNameOrNull()
                     );

                context.Result = new GrantValidationResult(
                    subject: loginResult.Identity.Claims.First(c=>c.Type== JwtRegisteredClaimNames.Sub).Value,
                    authenticationMethod: "passwrod",
                    claims: await CreateJwtClaims(loginResult)
                    );
            }
            catch (Exception e)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, e.Message);
            }
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

        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
        }


        private async Task<List<Claim>> CreateJwtClaims(AbpLoginResult<Tenant, User> loginResult)
        {

           
            var claims = loginResult.Identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == JwtRegisteredClaimNames.Sub);
             string userModel= JsonConvert.SerializeObject(Mapper.Map<UserLoginInfoDto>(loginResult.User));
            var org= await UserManager.GetOrganizationUnitsAsync(loginResult.User);
            string orgModel= JsonConvert.SerializeObject(Mapper.Map<List<OrgLoginInfo>>(org));
            var company=await CompanyManager.FindByIdAsync(loginResult.User.CompanyId);
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
    }
}
