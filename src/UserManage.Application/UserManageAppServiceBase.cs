using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Abp.Application.Services;
using Abp.IdentityFramework;
using Abp.Organizations;
using Abp.Runtime.Session;
using UserManage.AbpCompanyCore;
using UserManage.AbpCompanyCore.DomainService;
using UserManage.Authorization.Users;
using UserManage.MultiTenancy;
using UserManage.Authorization.Roles;

namespace UserManage
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class UserManageAppServiceBase : ApplicationService
    {
        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }

        public RoleManager RoleManager { get; set; }

        public AbpCompanyManager CompanyManager { get; set; }

        protected UserManageAppServiceBase()
        {
            LocalizationSourceName = UserManageConsts.LocalizationSourceName;
        }

        protected virtual Task<User> GetCurrentUserAsync()
        {
            var user = UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }

            return user;
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        protected virtual Task<AbpCompany> GetCurrentConpanyAsync()
        {
            var user = UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            return CompanyManager.FindByIdAsync(user.Result.CompanyId);
            //var company = AbpSession.GetCompany();
            //Task<AbpCompany> tCompany = Task.Run(() => company);
            //return tCompany;
        }

        protected virtual Task<List<OrganizationUnit>> GetCurrentOrgAsync(User user)
        {
            //var org = AbpSession.GetOrg();
            //Task<List<OrganizationUnit>> tOrg = Task.Run(() => org);
            //return tOrg;
            return UserManager.GetOrganizationUnitsAsync(user);
        }

        protected virtual async Task<string[]> GetRoleNames(User user)
        {

             var roles = await UserManager.GetRolesAsync(user);
            var rolename = (roles as List<string>).ToArray();
            return rolename;
        }
    }
}
