using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using UserManage.Roles.Dto;
using UserManage.Users.Dto;

namespace UserManage.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, GetUsersInput, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        Task ChangeLanguage(ChangeUserLanguageDto input);

        /// <summary>
        /// 根据用户名 ||用户ID && 公司名称  绑定公司ID
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task SetCompanyName(UserCompanyInputDto input);

        /// <summary>
        /// 分页查询用户（不含公司内容）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<UserListDto>> GetPagedWithoutCompany(GetUserInput input);

        /// <summary>
        /// 根据角色id查询用户列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ListResultDto<UserListDto>> GetUserListByRole(GetUserRoleInput input);
    }
}
