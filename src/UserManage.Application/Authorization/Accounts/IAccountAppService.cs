using System.Threading.Tasks;
using Abp.Application.Services;
using UserManage.Authorization.Accounts.Dto;

namespace UserManage.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
