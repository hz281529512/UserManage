
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
using Abp.Linq.Extensions;


using UserManage.BaseEntityCore;
using UserManage.BaseEntityCore.Dtos;
using UserManage.Authorization.Users;

namespace UserManage.BaseEntityCore
{
    /// <summary>
    /// BaseUserOrg应用层服务的接口实现方法  
    ///</summary>
    [AbpAuthorize]
    public class BaseUserOrgAppService : UserManageAppServiceBase, IBaseUserOrgAppService
    {
        private readonly IRepository<BaseUserOrg, int> _orgRepository;

        private readonly IRepository<BaseUserEmpOrg, int> _empOrgRepository;

        private readonly IRepository<User, long> _userRepository;

        private readonly IRepository<BaseUserEmp, long> _empRepository;

        /// <summary>
        /// 构造函数 
        ///</summary>
        public BaseUserOrgAppService(
            IRepository<BaseUserOrg, int> orgRepository,
            IRepository<BaseUserEmpOrg, int> empOrgRepository,
            IRepository<User, long> userRepository,
            IRepository<BaseUserEmp, long> empRepository
        )
        {
            _orgRepository = orgRepository;
            _empOrgRepository = empOrgRepository;
            _userRepository = userRepository;
            _empRepository = empRepository;
        }



        /// <summary>
        /// 获取组织
        /// </summary>

        public async Task<ListResultDto<BaseUserOrgDto>> GetOrganizationUnits()
        {
            var query =
                from ou in _orgRepository.GetAll()
                join uou in _empOrgRepository.GetAll() on ou.OrgGuid equals uou.DepartmentGuid into g
                select new { ou, memberCount = g.Count() };


            var items = await query.ToListAsync();
            return new ListResultDto<BaseUserOrgDto>(
                items.Select(item =>
                {
                    var dto = ObjectMapper.Map<BaseUserOrgDto>(item.ou);
                    dto.MemberCount = item.memberCount;
                    return dto;
                }).ToList());
        }

        /// <summary>
        /// 根据用户ID获取组织
        /// </summary>
        /// <param name="AbpUserId"></param>
        /// <returns></returns>
        public async Task<ListResultDto<BaseUserOrgDto>> GetOrganizationUnitsByUserId(long? AbpUserId)
        {
            var query =
                from ou in _orgRepository.GetAll()
                join uou in _empOrgRepository.GetAll() on ou.OrgGuid equals uou.DepartmentGuid 
                where uou.AbpUserId == AbpUserId
                select new { ou, uou };
            var items = await query.ToListAsync();
            return new ListResultDto<BaseUserOrgDto>(
                items.Select(item =>
                {
                    var dto = ObjectMapper.Map<BaseUserOrgDto>(item.ou);
                    dto.IsMaster = item.uou.IsMaster == "1";
                    return dto;
                }).ToList());
        }

        /// <summary>
        /// 获取组织用户
        /// </summary>
        public async Task<PagedResultDto<BaseUserEmpOrgDto>> GetOrganizationUnitUsers(GetBaseUserOrgsInput input)
        {
            var query = _empOrgRepository.GetAll();
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

            // var entityListDtos = ObjectMapper.Map<List<AbpCompanyListDto>>(entityList);
            //var entityListDtos = entityList.MapTo<List<BaseUserEmpOrgDto>>();

            return new PagedResultDto<BaseUserEmpOrgDto>(count, entityList.Select(item =>
            {
                var dto = item.MapTo<BaseUserEmpOrgDto>();
                if (dto.AbpUserId.HasValue)
                {
                    dto.AbpUserName = _userRepository.Get(dto.AbpUserId.Value)?.Name;
                }
                return dto;
            }).ToList());
        }
        /// <summary>
        /// 创建组织
        /// </summary>
        public async Task<BaseUserOrgListDto> CreateOrganizationUnit(BaseUserOrgEditDto input)
        {

            input.OrgGuid = Guid.NewGuid().ToString();
            var entity = input.MapTo<BaseUserOrg>();

            var new_id = await _orgRepository.InsertAndGetIdAsync(entity);
            entity.Id = new_id;
            return entity.MapTo<BaseUserOrgListDto>();
        }
        /// <summary>
        /// 更新组织
        /// </summary>
        public async Task<BaseUserOrgListDto> UpdateOrganizationUnit(BaseUserOrgEditDto input)
        {
            if (input.Id.HasValue)
            {

                var entity = await _orgRepository.GetAsync(input.Id.Value);

                await _orgRepository.UpdateAsync(entity);

                return entity.MapTo<BaseUserOrgListDto>();
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// 删除组织
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteOrganizationUnit(EntityDto<int> input)
        {
            await _orgRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 移除用户对应的组织
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task RemoveUserFromOrganizationUnit(UserToOrgInput input)
        {
            //await UserManager.RemoveFromOrganizationUnitAsync(input.UserId, input.OrganizationUnitId);
            if (input.AbpUserId.HasValue && !string.IsNullOrEmpty(input.OrgGuid))
            {
                await _empOrgRepository.DeleteAsync(x => x.DepartmentGuid == input.OrgGuid && x.AbpUserId == input.AbpUserId);
            }
        }

        /// <summary>
        /// 根据用户ID 添加 组织列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task AddUsersToOrganizationUnit(OrgsToUserInput input)
        {
            var emp = await _empRepository.FirstOrDefaultAsync(x => x.AbpUserId == input.AbpUserId);
            foreach (var item in input.OrgGuids)
            {
                var org = await _orgRepository.FirstOrDefaultAsync(x => x.OrgGuid == item);

                await _empOrgRepository.InsertAsync(new BaseUserEmpOrg
                {
                    AbpUserId = emp.AbpUserId,
                    BaseUserGuid = Guid.NewGuid().ToString(),
                    EmpUserGuid = emp.EmpUserGuid,
                    CropId = "wx003757ee144cae06",
                    EmpUserId = emp.EmpUserId,
                    IsMaster = item == input.MasterOrgGuid ? "1" : "0",
                });
            }
        }


        //public async Task<PagedResultDto<NameValueDto>> FindUsers(FindOrganizationUnitUsersInput input)
        //{
        //    var userIdsInOrganizationUnit = _userOrganizationUnitRepository.GetAll()
        //        .Where(uou => uou.OrganizationUnitId == input.OrganizationUnitId)
        //        .Select(uou => uou.UserId);

        //    var query = UserManager.Users
        //        .Where(u => !userIdsInOrganizationUnit.Contains(u.Id))
        //        .WhereIf(
        //            !input.Filter.IsNullOrWhiteSpace(),
        //            u =>
        //                u.Name.Contains(input.Filter) ||
        //                u.Surname.Contains(input.Filter) ||
        //                u.UserName.Contains(input.Filter) ||
        //                u.EmailAddress.Contains(input.Filter)
        //        );

        //    var userCount = await query.CountAsync();
        //    var users = await query
        //        .OrderBy(u => u.Name)
        //        .ThenBy(u => u.Surname)
        //        .PageBy(input)
        //        .ToListAsync();

        //    return new PagedResultDto<NameValueDto>(
        //        userCount,
        //        users.Select(u =>
        //            new NameValueDto(
        //                u.FullName + " (" + u.EmailAddress + ")",
        //                u.Id.ToString()
        //            )
        //        ).ToList()
        //    );
        //}

        //private async Task<OrganizationUnitDto> CreateOrganizationUnitDto(OrganizationUnit organizationUnit)
        //{
        //    var dto = ObjectMapper.Map<OrganizationUnitDto>(organizationUnit);
        //    dto.MemberCount = await _userOrganizationUnitRepository.CountAsync(uou => uou.OrganizationUnitId == organizationUnit.Id);
        //    return dto;
        //}

    }
}


