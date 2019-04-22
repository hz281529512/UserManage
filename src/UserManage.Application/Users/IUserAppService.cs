using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using UserManage.Roles.Dto;
using UserManage.Users.Dto;

namespace UserManage.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        Task ChangeLanguage(ChangeUserLanguageDto input);
    }
}
