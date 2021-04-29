using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Auditing;
using Microsoft.AspNetCore.Mvc;
using UserManage.Sessions.Dto;

namespace UserManage.Sessions
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SessionAppService : UserManageAppServiceBase, ISessionAppService
    {
        [DisableAuditing]
        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations()
        {
            var output = new GetCurrentLoginInformationsOutput
            {
                Application = new ApplicationInfoDto
                {
                    Version = AppVersionHelper.Version,
                    ReleaseDate = AppVersionHelper.ReleaseDate,
                    Features = new Dictionary<string, bool>()
                }
            };

            if (AbpSession.TenantId.HasValue)
            {
                output.Tenant = ObjectMapper.Map<TenantLoginInfoDto>(await GetCurrentTenantAsync());
            }

            if (AbpSession.UserId.HasValue)
            {
                var user = await GetCurrentUserAsync();
                output.User = ObjectMapper.Map<UserLoginInfoDto>(user);
                output.Company = ObjectMapper.Map<CompanyLoginInfo>(await GetCurrentConpanyAsync());
                var org = await GetCurrentOrgAsync(user);
                output.ListOrg = ObjectMapper.Map<List<OrgLoginInfo>>(org);
                output.RoleNames = await GetRoleNames(user);
                output.RoleIds = await GetRoleIds(user);
            }

            return output;
        }
    }
}
