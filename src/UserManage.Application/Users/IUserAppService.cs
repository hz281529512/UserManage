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
        /// �����û��� ||�û�ID && ��˾����  �󶨹�˾ID
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task SetCompanyName(UserCompanyInputDto input);

        /// <summary>
        /// ��ҳ��ѯ�û���������˾���ݣ�
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<UserListDto>> GetPagedWithoutCompany(GetUserInput input);

        /// <summary>
        /// ���ݽ�ɫid��ѯ�û��б�
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ListResultDto<UserListDto>> GetUserListByRole(GetUserRoleInput input);
    }
}
