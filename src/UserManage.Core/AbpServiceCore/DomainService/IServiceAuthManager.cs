

using System;
using System.Threading.Tasks;
using Abp;
using Abp.Domain.Services;
using UserManage.AbpServiceCore;


namespace UserManage.AbpServiceCore.DomainService
{
    public interface IServiceAuthManager : IDomainService
    {
        Task<AuthUserInfo> GetUserInfo(string provider, string accessCode);

        Task<string> GetToken(string local_name, string CropId, string Secret);
    }
}
