
using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using Abp.UI;
using Abp.AutoMapper;
using Abp.Extensions;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.Linq.Extensions;


using UserManage.BaseEntityCore;
using UserManage.BaseEntityCore.Dtos;
using UserManage.Authorization.Users;

namespace UserManage.BaseEntityCore
{
    /// <summary>
    /// BaseUserEmpRole应用层服务的接口实现方法  
    ///</summary>
    //[AbpAuthorize]
    public class BaseUserEmpRoleAppService : UserManageAppServiceBase, IBaseUserEmpRoleAppService
    {
        private readonly IRepository<BaseUserEmp, long> _empRepository;

        private readonly IRepository<BaseUserEmpRole, int> _empRoleRepository;

        private readonly IRepository<BaseUserRole, int> _roleRepository;

        private readonly IRepository<User, long> _userRepository;

        private readonly IRepository<UserLogin, long> _userLoginRepository;
        /// <summary>
        /// 构造函数 
        ///</summary>
        public BaseUserEmpRoleAppService(
            IRepository<BaseUserEmp, long> empRepository,
            IRepository<BaseUserEmpRole, int> empRoleRepository,
            IRepository<BaseUserRole, int> roleRepository,
            IRepository<User, long> userRepository,
            IRepository<UserLogin, long> userLoginRepository)
        {
            _empRepository = empRepository;
            _empRoleRepository = empRoleRepository;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _userLoginRepository = userLoginRepository;
        }


        /// <summary>
        /// 获取BaseUserEmpRole的分页列表信息
        ///</summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task<PagedResultDto<BaseUserEmpRoleListDto>> GetPaged(GetBaseUserEmpRolesInput input)
        {

            var query = _empRoleRepository.GetAll();
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

            // var entityListDtos = ObjectMapper.Map<List<BaseUserEmpRoleListDto>>(entityList);
            var entityListDtos = entityList.MapTo<List<BaseUserEmpRoleListDto>>();

            return new PagedResultDto<BaseUserEmpRoleListDto>(count, entityListDtos);
        }

        /// <summary>
        /// 获取Base User 角色列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ListResultDto<BaseUserRoleListDto>> GetRoleList(GetBaseUserRolesInput input)
        {
            var query = _roleRepository.GetAll();
            // TODO:根据传入的参数添加过滤条件
            if (!string.IsNullOrEmpty(input.Filter))
            {
                query = query.Where(input.Filter);
            }
            if (input.AbpUserId.HasValue)
            {
                query = query.Where(x => _empRoleRepository.GetAll().Any(r => r.AbpUserId == input.AbpUserId.ToString() && x.Id == r.BaseRoleId));
            }

            var entityList = await query
                    .OrderBy(input.Sorting).AsNoTracking()
                    .ToListAsync();

            // var entityListDtos = ObjectMapper.Map<List<BaseUserEmpRoleListDto>>(entityList);
            var entityListDtos = entityList.MapTo<List<BaseUserRoleListDto>>();

            return new ListResultDto<BaseUserRoleListDto>(entityListDtos);
        }

        /// <summary>
        /// 根据角色id批量获取用户
        /// </summary>
        /// <param name="role_id"></param>
        /// <returns></returns>
        public async Task<ListResultDto<BaseEmpDto>> GetUserListByRoleId(int role_id)
        {
            var query = from emp in _empRepository.GetAll().AsNoTracking()
                        join u in _userRepository.GetAll().AsNoTracking() on emp.AbpUserId equals u.Id
                        where _empRoleRepository.GetAll().Any(r => r.BaseRoleId == role_id && r.AbpUserId == u.Id.ToString())
                        select new BaseEmpDto
                        {
                            AbpUserId = emp.AbpUserId,
                            EmpOrderNo = emp.EmpOrderNo,
                            EmpStationId = emp.EmpStationId,
                            EmpStatus = emp.EmpStatus,
                            EmpUserGuid = emp.EmpUserGuid,
                            EmpUserId = emp.EmpUserId,
                            IsLeader = emp.IsLeader,
                            Name = u.Name
                        };
            var entity_list = await query.ToListAsync();
            return new ListResultDto<BaseEmpDto>(entity_list);
        }

        /// <summary>
        /// 根据abp用户id批量更新 base user 角色ID
        /// </summary>
        /// <param name="AbpUserId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public async Task BatchUpdateEmpByRolesAsync(long AbpUserId, List<int> roles)
        {
            if (AbpUserId == 0)
            {
                throw new UserFriendlyException("AbpUserId 不能为0");
            }
            var str_user_id = AbpUserId.ToString();
            await _empRoleRepository.DeleteAsync(x => x.AbpUserId == str_user_id);
            if (roles?.Count > 0)
            {
                var emp_user = await _empRepository.FirstOrDefaultAsync(x => x.AbpUserId == AbpUserId);
                if (emp_user != null)
                {
                    foreach (var item in roles)
                    {
                        await _empRoleRepository.InsertAsync(new BaseUserEmpRole
                        {
                            AbpUserId = str_user_id,
                            BaseRoleId = item,
                            BaseUniqueId = Guid.NewGuid().ToString(),
                            EmpUserGuid = emp_user.EmpUserGuid,
                            EmpUserId = emp_user.EmpUserId
                        });
                    }
                }
            }
        }

        /// <summary>
        /// 根据企业微信id批量更新 base user 角色ID
        /// </summary>
        /// <param name="AbpUserId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public async Task BatchUpdateWxByRolesAsync(string WXId, List<int> roles)
        {
            var current_tenantid = 2;
            
            using (CurrentUnitOfWork.SetTenantId(current_tenantid))
            {
                if (!WXId.IsNullOrEmpty())
                {
                    throw new UserFriendlyException("AbpUserId 不能为0");
                }

                var abpUser =  _userLoginRepository.FirstOrDefault(x =>
                    x.ProviderKey == WXId && x.LoginProvider == "Wechat");
                if(abpUser  == null) throw new UserFriendlyException("无法操作未绑定");
                var str_user_id = abpUser.Id.ToString();
                await _empRoleRepository.DeleteAsync(x => x.AbpUserId == str_user_id);
                if (roles?.Count > 0)
                {
                    var emp_user = await _empRepository.FirstOrDefaultAsync(x => x.AbpUserId == abpUser.Id);
                    if (emp_user != null)
                    {
                        foreach (var item in roles)
                        {
                            await _empRoleRepository.InsertAsync(new BaseUserEmpRole
                            {
                                AbpUserId = str_user_id,
                                BaseRoleId = item,
                                BaseUniqueId = Guid.NewGuid().ToString(),
                                EmpUserGuid = emp_user.EmpUserGuid,
                                EmpUserId = emp_user.EmpUserId
                            });
                        }
                    }
                }
            }
        }


        //public async Task

        /// <summary>
        /// 通过指定id获取BaseUserEmpRoleListDto信息
        /// </summary>

        public async Task<BaseUserEmpRoleListDto> GetById(EntityDto<int> input)
        {
            var entity = await _empRoleRepository.GetAsync(input.Id);

            return entity.MapTo<BaseUserEmpRoleListDto>();
        }

        /// <summary>
        /// 获取编辑 BaseUserEmpRole
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task<GetBaseUserEmpRoleForEditOutput> GetForEdit(NullableIdDto<int> input)
        {
            var output = new GetBaseUserEmpRoleForEditOutput();
            BaseUserEmpRoleEditDto editDto;

            if (input.Id.HasValue)
            {
                var entity = await _empRoleRepository.GetAsync(input.Id.Value);

                editDto = entity.MapTo<BaseUserEmpRoleEditDto>();

                //baseUserEmpRoleEditDto = ObjectMapper.Map<List<baseUserEmpRoleEditDto>>(entity);
            }
            else
            {
                editDto = new BaseUserEmpRoleEditDto();
            }

            output.BaseUserEmpRole = editDto;
            return output;
        }


        /// <summary>
        /// 添加或者修改BaseUser 角色的公共方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task<BaseUserRoleEditDto> CreateOrUpdate(CreateOrUpdateBaseUserRoleInput input)
        {

            if (input.BaseUserRole.Id.HasValue)
            {
                return await Update(input.BaseUserRole);
            }
            else
            {
                return await Create(input.BaseUserRole);
            }
        }


        /// <summary>
        /// 新增BaseUserEmpRole
        /// </summary>

        protected virtual async Task<BaseUserRoleEditDto> Create(BaseUserRoleEditDto input)
        {
            //TODO:新增前的逻辑判断，是否允许新增

            // var entity = ObjectMapper.Map <BaseUserEmpRole>(input);
            var entity = input.MapTo<BaseUserRole>();

            var new_id = await _roleRepository.InsertAndGetIdAsync(entity);
            entity.Id = new_id;
            return entity.MapTo<BaseUserRoleEditDto>();
        }

        /// <summary>
        /// 编辑BaseUserEmpRole
        /// </summary>

        protected virtual async Task<BaseUserRoleEditDto> Update(BaseUserRoleEditDto input)
        {
            //TODO:更新前的逻辑判断，是否允许更新

            var entity = await _roleRepository.GetAsync(input.Id.Value);
            input.MapTo(entity);

            // ObjectMapper.Map(input, entity);
            await _roleRepository.UpdateAsync(entity);
            return input;
        }



        /// <summary>
        /// 删除BaseUserEmpRole信息的方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task DeleteRole(EntityDto<int> input)
        {
            //TODO:删除前的逻辑判断，是否允许删除
            await _roleRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 根据baseuser 角色Id 与abp 用户id 移除关系
        /// </summary>
        /// <param name="role_id"></param>
        /// <param name="AbpUserIds"></param>
        /// <returns></returns>
        public async Task RemoveEmpRoleByAbpUserIds(int role_id, List<string> AbpUserIds)
        {
            if (AbpUserIds?.Count > 0)
            {
                await _empRoleRepository.DeleteAsync(s => s.BaseRoleId == role_id && AbpUserIds.Contains(s.AbpUserId));
            }
        }

        /// <summary>
        /// 根据baseuser 角色Id 与abp 用户id 移除关系(批量)
        /// </summary>
        /// <param name="role_id"></param>
        /// <param name="AbpUserIds"></param>
        /// <returns></returns>
        public async Task BatchUpdateEmpRoleByAbpUserIds(int role_id, List<int> AbpUserIds)
        {
            if (AbpUserIds?.Count > 0)
            {
                await _empRoleRepository.DeleteAsync(s => s.BaseRoleId == role_id);

                foreach (var item in AbpUserIds)
                {
                    var emp_user = await _empRepository.FirstOrDefaultAsync(x => x.AbpUserId == item);
                    if (emp_user != null)
                    {
                        await _empRoleRepository.InsertAsync(new BaseUserEmpRole
                        {
                            AbpUserId = item.ToString(),
                            BaseRoleId = role_id,
                            BaseUniqueId = Guid.NewGuid().ToString(),
                            EmpUserGuid = emp_user.EmpUserGuid,
                            EmpUserId = emp_user.EmpUserId
                        });
                    }
                }
            }
        }


        /// <summary>
        /// 批量删除BaseUserEmpRole的方法
        /// </summary>

        public async Task BatchDelete(List<int> input)
        {
            // TODO:批量删除前的逻辑判断，是否允许删除
            await _empRoleRepository.DeleteAsync(s => input.Contains(s.Id));
        }


        /// <summary>
        /// 导出BaseUserEmpRole为excel表,等待开发。
        /// </summary>
        /// <returns></returns>
        //public async Task<FileDto> GetToExcel()
        //{
        //	var users = await UserManager.Users.ToListAsync();
        //	var userListDtos = ObjectMapper.Map<List<UserListDto>>(users);
        //	await FillRoleNames(userListDtos);
        //	return _userListExcelExporter.ExportToFile(userListDtos);
        //}

    }
}


