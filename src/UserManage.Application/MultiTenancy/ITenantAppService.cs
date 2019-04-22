using Abp.Application.Services;
using Abp.Application.Services.Dto;
using UserManage.MultiTenancy.Dto;

namespace UserManage.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

