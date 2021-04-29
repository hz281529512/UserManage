using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.UI;
using UserManage.Authorization;
using UserManage.Authorization.Accounts;
using UserManage.Authorization.Roles;
using UserManage.Authorization.Users;
using UserManage.Roles.Dto;
using UserManage.Users.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManage.AbpCompanyCore;
using System;
using System.Linq.Dynamic.Core;
using Abp.Authorization.Users;

namespace UserManage.Users
{   
    /// <summary>
    /// 用户
    /// </summary>
    [AbpAuthorize]
    public class UserAppService : AsyncCrudAppService<User, UserDto, long, GetUsersInput, CreateUserDto, UserDto>, IUserAppService
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IAbpSession _abpSession;
        private readonly LogInManager _logInManager;
        private readonly IRepository<AbpCompany, Guid> _companyRepository;

        public UserAppService(
            IRepository<User, long> repository,
            UserManager userManager,
            RoleManager roleManager,
            IRepository<Role> roleRepository,
            IPasswordHasher<User> passwordHasher,
            IAbpSession abpSession,
            LogInManager logInManager,
            IRepository<AbpCompany, Guid> companyRepository,
            IRepository<UserRole, long> userRoleRepository)
            : base(repository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _abpSession = abpSession;
            _logInManager = logInManager;
            _companyRepository = companyRepository;
            _userRoleRepository = userRoleRepository;
        }
        /// <summary>
        /// 用户分页查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<UserListDto>> GetPaged(GetUserInput input)
        {
            try
            {

                var query = from u in Repository.GetAllIncluding(x => x.Roles)
                            join c in _companyRepository.GetAll() on u.CompanyId equals c.Id.ToString()
                            select new { u, c };

                // TODO:根据传入的参数添加过滤条件

                if (!string.IsNullOrEmpty(input.Filter))
                {
                    query = query.Where(input.Filter);
                }
                var count = await query.CountAsync();

                var entityList = await query
                    .OrderBy(input.Sorting).AsNoTracking()
                    .PageBy(input)
                    .ToListAsync();

                return new PagedResultDto<UserListDto>(count, entityList.Select(item =>
                {
                    var roles = _roleManager.Roles.Where(r => item.u.Roles.Any(ur => ur.RoleId == r.Id)).Select(r => r.NormalizedName);
                    var dto = ObjectMapper.Map<UserListDto>(item.u);
                    dto.RoleNames = roles.ToArray();
                    dto.AbpCompany = item.c;
                    return dto;
                }).ToList());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<UserDto> Create(CreateUserDto input)
        {
            CheckCreatePermission();

            var user = ObjectMapper.Map<User>(input);

            user.TenantId = AbpSession.TenantId;
            user.IsEmailConfirmed = true;

            await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

            CheckErrors(await _userManager.CreateAsync(user, input.Password));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRoles(user, input.RoleNames));
            }

            CurrentUnitOfWork.SaveChanges();

            return MapToEntityDto(user);
        }
        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<UserDto> Update(UserDto input)
        {
            try
            {
                CheckUpdatePermission();

                var user = await _userManager.GetUserByIdAsync(input.Id);

                MapToEntity(input, user);

                CheckErrors(await _userManager.UpdateAsync(user));

                if (input.RoleNames != null)
                {
                    await Repository.EnsureCollectionLoadedAsync(user, u => u.Roles);

                    foreach (var userRole in user.Roles.ToList())
                    {

                        var role = await _roleManager.FindByIdAsync(userRole.RoleId.ToString());
                        if (input.RoleNames.All(roleName => role.Name != roleName))
                        {

                            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                        }
                    }

                    //Add to added roles
                    foreach (var roleName in input.RoleNames)
                    {
                        var role = await _roleManager.GetRoleByNameAsync(roleName);
                        if (user.Roles.All(ur => ur.RoleId != role.Id))
                        {
                            var result = await _userManager.AddToRoleAsync(user, roleName);
                        }
                    }

                    //CheckErrors(await _userManager.SetRoles(user, input.RoleNames));
                }

                return await Get(input);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        //public override async Task Delete(EntityDto<long> input)
        //{
        //    var user = await _userManager.GetUserByIdAsync(input.Id);
        //    await _userManager.DeleteAsync(user);
        //}
        /// <summary>
        /// 获取角色
        /// </summary>
        /// <returns></returns>
        public async Task<ListResultDto<RoleDto>> GetRoles()
        {
            var roles = await _roleRepository.GetAllListAsync();
            return new ListResultDto<RoleDto>(ObjectMapper.Map<List<RoleDto>>(roles));
        }
        /// <summary>
        /// 改变语言
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task ChangeLanguage(ChangeUserLanguageDto input)
        {
            await SettingManager.ChangeSettingForUserAsync(
                AbpSession.ToUserIdentifier(),
                LocalizationSettingNames.DefaultLanguage,
                input.LanguageName
            );
        }

        protected override User MapToEntity(CreateUserDto createInput)
        {
            var user = ObjectMapper.Map<User>(createInput);
            user.SetNormalizedNames();
            return user;
        }

        protected override void MapToEntity(UserDto input, User user)
        {
            ObjectMapper.Map(input, user);
            user.SetNormalizedNames();
        }

        protected override UserDto MapToEntityDto(User user)
        {
            var roles = _roleManager.Roles.Where(r => user.Roles.Any(ur => ur.RoleId == r.Id)).Select(r => r.NormalizedName);
            var userDto = base.MapToEntityDto(user);
            userDto.RoleNames = roles.ToArray();
            return userDto;
        }

        protected override IQueryable<User> CreateFilteredQuery(GetUsersInput input)
        {
            var query = Repository.GetAllIncluding(x => x.Roles);
            if (!string.IsNullOrEmpty(input.Where))
            {
                return query.Where(input.Where);
            }
            return query;
        }

        protected override async Task<User> GetEntityByIdAsync(long id)
        {
            var user = await Repository.GetAllIncluding(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                throw new EntityNotFoundException(typeof(User), id);
            }

            return user;
        }

        protected override IQueryable<User> ApplySorting(IQueryable<User> query, GetUsersInput input)
        {
            return query.OrderBy(r => r.UserName);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
        /// <summary>
        /// 改变密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> ChangePassword(ChangePasswordDto input)
        {
            if (_abpSession.UserId == null)
            {
                throw new UserFriendlyException("Please log in before attemping to change password.");
            }
            long userId = _abpSession.UserId.Value;
            var user = await _userManager.GetUserByIdAsync(userId);
            var loginAsync = await _logInManager.LoginAsync(user.UserName, input.CurrentPassword, shouldLockout: false);
            if (loginAsync.Result != AbpLoginResultType.Success)
            {
                throw new UserFriendlyException("Your 'Existing Password' did not match the one on record.  Please try again or contact an administrator for assistance in resetting your password.");
            }
            if (!new Regex(AccountAppService.PasswordRegex).IsMatch(input.NewPassword))
            {
                throw new UserFriendlyException("Passwords must be at least 8 characters, contain a lowercase, uppercase, and number.");
            }
            user.Password = _passwordHasher.HashPassword(user, input.NewPassword);
            CurrentUnitOfWork.SaveChanges();
            return true;
        }
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> ResetPassword(ResetPasswordDto input)
        {
            if (_abpSession.UserId == null)
            {
                throw new UserFriendlyException("Please log in before attemping to reset password.");
            }
            long currentUserId = _abpSession.UserId.Value;
            var currentUser = await _userManager.GetUserByIdAsync(currentUserId);
            var loginAsync = await _logInManager.LoginAsync(currentUser.UserName, input.AdminPassword, shouldLockout: false);
            if (loginAsync.Result != AbpLoginResultType.Success)
            {
                throw new UserFriendlyException("Your 'Admin Password' did not match the one on record.  Please try again.");
            }
            if (currentUser.IsDeleted || !currentUser.IsActive)
            {
                return false;
            }
            var roles = await _userManager.GetRolesAsync(currentUser);
            if (!roles.Contains(StaticRoleNames.Tenants.Admin))
            {
                throw new UserFriendlyException("Only administrators may reset passwords.");
            }

            var user = await _userManager.GetUserByIdAsync(input.UserId);
            if (user != null)
            {
                user.Password = _passwordHasher.HashPassword(user, input.NewPassword);
                CurrentUnitOfWork.SaveChanges();
            }

            return true;
        }


        /// <summary>
        /// 绑定公司
        /// </summary>
        /// <param name="input">根据用户名 ||用户ID && 公司名称  绑定公司ID</param>
        /// <returns></returns>
        public async Task SetCompanyName(UserCompanyInputDto input)
        {
            User entity = null;
            long userid = input.UserId ?? 0;
            if (userid != 0)
            {

                entity = await _userManager.GetUserByIdAsync(input.UserId.Value);
            }
            else if (!string.IsNullOrEmpty(input.UserName)) { entity = await _userManager.FindByNameAsync(input.UserName); }

            if (entity == null) throw new EntityNotFoundException(typeof(User), input.UserId);
            if (!string.IsNullOrEmpty(entity.CompanyId)) throw new UserFriendlyException("该用户已绑定公司!");

            var company_entity = await _companyRepository.GetAll().FirstOrDefaultAsync(x => x.CompanyName == input.CompanyName);
            if (company_entity == null)
            {
                throw new UserFriendlyException("公司名无效!");
            }
            entity.CompanyId = company_entity.Id.ToString();
            CheckErrors(await _userManager.UpdateAsync(entity));
        }

        /// <summary>
        /// 分页查询用户（不含公司内容）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<UserListDto>> GetPagedWithoutCompany(GetUserInput input)
        {
            var query = from u in Repository.GetAllIncluding(x => x.Roles) select new { u };
            if (!string.IsNullOrEmpty(input.Filter))
            {
                query = query.Where(input.Filter);
            }
            var count = await query.CountAsync();

            var entityList = await query
                .OrderBy(input.Sorting).AsNoTracking()
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<UserListDto>(count, entityList.Select(item =>
            {
                var roles = _roleManager.Roles.Where(r => item.u.Roles.Any(ur => ur.RoleId == r.Id)).Select(r => r.NormalizedName);
                var dto = ObjectMapper.Map<UserListDto>(item.u);
                dto.RoleNames = roles.ToArray();
                return dto;
            }).ToList());
        }

        /// <summary>
        /// 根据角色id查询用户列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ListResultDto<UserListDto>> GetUserListByRole(GetUserRoleInput input)
        {
            var query = from u in Repository.GetAllIncluding(x => x.Roles) select new { u };

            if (input.RoleId.HasValue)
            {
                query = query.Where(x => x.u.Roles.Any(r => r.RoleId == input.RoleId));
            }

            if (!string.IsNullOrEmpty(input.Filter))
            {
                query = query.Where(input.Filter);
            }

            var entityList = await query
                .OrderBy(input.Sorting).AsNoTracking()
                .ToListAsync();

            return new ListResultDto<UserListDto>(entityList.Select(item =>
            {
                var roles = _roleManager.Roles.Where(r => item.u.Roles.Any(ur => ur.RoleId == r.Id)).Select(r => r.NormalizedName);
                var dto = ObjectMapper.Map<UserListDto>(item.u);
                dto.RoleNames = roles.ToArray();
                return dto;
            }).ToList());
        }

        /// <summary>
        /// 批量修改角色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task BatchUpdateRoles(List<BatchUserDto> input)
        {
            CheckUpdatePermission();
            foreach (var item in input)
            {
                var user = await _userManager.GetUserByIdAsync(item.UserId);
                if (item.RoleNames != null)
                {
                    CheckErrors(await _userManager.SetRoles(user, item.RoleNames));
                }
            }
        }

        /// <summary>
        /// 批量修改角色关联用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task BatchUpdateRoleUsers(BatchRoleUsers input)
        {
            CheckUpdatePermission();
            if (input.Userids.Any())
            {
                var tenant_id = this.AbpSession.TenantId;
                if (!tenant_id.HasValue) throw new UserFriendlyException("无效租户！");
                await _userRoleRepository.DeleteAsync(x => x.RoleId == input.RoleId && x.TenantId == tenant_id);
                foreach (var item in input.Userids)
                {
                    await _userRoleRepository.InsertAsync(new UserRole { RoleId = input.RoleId, TenantId = tenant_id, UserId = item });
                }
            }
        }

        /// <summary>
        /// 批量修改用户地区
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task BatchUpdateDistrict(List<BatchUserDistrictInputDto> input)
        {
            CheckUpdatePermission();
            foreach (var item in input)
            {
                var user = await _userManager.GetUserByIdAsync(item.UserId);
                //拼接用户地区
                user.SelectDistrict = item.Districts == null ? "" : string.Join(',', item.Districts);
                CheckErrors(await _userManager.UpdateAsync(user));
            }
        }

        /// <summary>
        /// 获取当前负责人旗下客户限额
        /// </summary>
        /// <returns></returns>
        public async Task<ListResultDto<UserClientQuotaDto>> GetAmClientQuota(string am = "",string district = "")
        {

            var current_user = await _userManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            var select_district = string.IsNullOrEmpty(current_user.SelectDistrict) ? "" : current_user.SelectDistrict;

            if (select_district.IsNullOrEmpty())
            {
                throw new UserFriendlyException(99, "当前用户并未设置地区");
            }
            if (!district.IsNullOrEmpty())
            {
                if(select_district.Contains(district))
                    select_district = district;
                else
                    throw new UserFriendlyException(99, "当前用户不支持目标地区");
            }
            var districts = select_district.Split(',');
            var d = districts.AsQueryable();
            //var user_query = Repository.GetAll().Where(x => districts.Any(a => x.SelectDistrict.Contains(a)));
            //" @0.Contains(" + dictrict_name + ")", dCondiction
            IEnumerable<User> u_list = new List<User>();
            foreach (var item in districts)
            {
                var user_query = Repository.GetAll();//.Where(" @0.Contains(SelectDistrict)", select_district);
                if (!am.IsNullOrEmpty())
                {
                    user_query = user_query.Where(x => x.Name == am);
                }
                //if (!district.IsNullOrEmpty())
                //{
                //    user_query = user_query.Where(x => x.SelectDistrict.Contains(district));
                //}
                //var user_query = UserManager.Users.FilterDistrictQuery(current_user, 1, "SelectDistrict");
                var role1 = await _roleManager.FindByNameAsync("分公司客户经理");
                var role2 = await _roleManager.FindByNameAsync("WX_客户经理");
                var u_query = from u in user_query.AsNoTracking()
                              join ur in _userRoleRepository.GetAll().AsNoTracking() on u.Id equals ur.UserId
                              where u.SelectDistrict.Contains(item) && ur.RoleId.IsIn(role1.Id, role2.Id)
                              select u;

                if (u_query.Any()) u_list = u_list.Union(u_query.ToList());
            }

            u_list = u_list.Distinct().ToList();

            return new ListResultDto<UserClientQuotaDto>(u_list.Select(item => new UserClientQuotaDto
            {
                UserId = item.Id,
                Name = item.Name,
                ClientQuota = item.ClientQuota,
            }).ToList());
        }

        /// <summary>
        /// 更新客户数量限制
        /// </summary>
        /// <returns></returns>
        public async Task<UserDto> UpdateClientQuota(UserClientQuotaDto input)
        {
            var user = await _userManager.GetUserByIdAsync(input.UserId);
            user.ClientQuota = input.ClientQuota;
            await _userManager.UpdateAsync(user);
            return ObjectMapper.Map<UserDto>(user);
        }
    }
}

