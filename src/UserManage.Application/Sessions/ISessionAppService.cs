using System.Threading.Tasks;
using Abp.Application.Services;
using UserManage.Sessions.Dto;

namespace UserManage.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
