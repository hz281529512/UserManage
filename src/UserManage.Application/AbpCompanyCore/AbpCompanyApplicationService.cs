
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


using UserManage.AbpCompanyCore;
using UserManage.AbpCompanyCore.Dtos;
using UserManage.AbpCompanyCore.DomainService;



namespace UserManage.AbpCompanyCore
{
    /// <summary>
    /// AbpCompany应用层服务的接口实现方法  
    ///</summary>
    [AbpAuthorize]
    public class AbpCompanyAppService : UserManageAppServiceBase, IAbpCompanyAppService
    {
        private readonly IRepository<AbpCompany, Guid> _entityRepository;

        private readonly AbpCompanyManager _entityManager;

        /// <summary>
        /// 构造函数 
        ///</summary>
        public AbpCompanyAppService(
        IRepository<AbpCompany, Guid> entityRepository
        , AbpCompanyManager entityManager
        )
        {
            _entityRepository = entityRepository;
            _entityManager = entityManager;
        }


        /// <summary>
        /// 获取AbpCompany的分页列表信息
        ///</summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task<PagedResultDto<AbpCompanyListDto>> GetPaged(GetAbpCompanysInput input)
        {

            var query = _entityRepository.GetAll();
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
            var entityListDtos = entityList.MapTo<List<AbpCompanyListDto>>();

            return new PagedResultDto<AbpCompanyListDto>(count, entityListDtos);
        }


        /// <summary>
        /// 通过指定id获取AbpCompanyListDto信息
        /// </summary>

        public async Task<AbpCompanyListDto> GetById(EntityDto<Guid> input)
        {
            var entity = await _entityRepository.GetAsync(input.Id);

            return entity.MapTo<AbpCompanyListDto>();
        }

        /// <summary>
        /// 获取编辑 AbpCompany
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task<GetAbpCompanyForEditOutput> GetForEdit(NullableIdDto<Guid> input)
        {
            var output = new GetAbpCompanyForEditOutput();
            AbpCompanyEditDto editDto;

            if (input.Id.HasValue)
            {
                var entity = await _entityRepository.GetAsync(input.Id.Value);

                editDto = entity.MapTo<AbpCompanyEditDto>();

                //abpCompanyEditDto = ObjectMapper.Map<List<abpCompanyEditDto>>(entity);
            }
            else
            {
                editDto = new AbpCompanyEditDto();
            }

            output.AbpCompany = editDto;
            return output;
        }

        /// <summary>
        /// 获取登录用户公司
        /// </summary>
        public async Task<AbpCompanyListDto> GetCompanyUser()
        {
            var users = UserManager.Users.First(c => c.Id == AbpSession.UserId);
            if (!string.IsNullOrEmpty(users.CompanyId))
            {
                Guid companyid = new Guid(users.CompanyId);
                var entity = await _entityRepository.GetAsync(companyid);
                return entity.MapTo<AbpCompanyListDto>();
            }

            return null;

        }
        /// <summary>
        /// 添加或者修改AbpCompany的公共方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task<AbpCompanyEditDto> CreateOrUpdate(CreateOrUpdateAbpCompanyInput input)
        {

            if (input.AbpCompany.Id.HasValue)
            {
                return await Update(input.AbpCompany);
            }
            else
            {
                return await Create(input.AbpCompany);
            }
        }


        /// <summary>
        /// 新增AbpCompany
        /// </summary>

        protected virtual async Task<AbpCompanyEditDto> Create(AbpCompanyEditDto input)
        {
            //TODO:新增前的逻辑判断，是否允许新增

            // var entity = ObjectMapper.Map <AbpCompany>(input);
            var entity = input.MapTo<AbpCompany>();
            var company = _entityRepository.GetAll().FirstOrDefault(c => c.CompanyNo == entity.CompanyNo);
            if (company == null)
            {
                //entity.Code = Guid.NewGuid().ToString().Replace("-", "").ToUpper().Substring(0, 12);
                entity = await _entityManager.CreateAsync(entity);
                return entity.MapTo<AbpCompanyEditDto>();
            }
            return company.MapTo<AbpCompanyEditDto>();

        }

        /// <summary>
        /// 编辑AbpCompany
        /// </summary>

        protected virtual async Task<AbpCompanyEditDto> Update(AbpCompanyEditDto input)
        {
            //TODO:更新前的逻辑判断，是否允许更新

            var entity = await _entityRepository.GetAsync(input.Id.Value);
            input.MapTo(entity);

            // ObjectMapper.Map(input, entity);
            await _entityRepository.UpdateAsync(entity);
            return ObjectMapper.Map<AbpCompanyEditDto>(entity);
        }



        /// <summary>
        /// 删除AbpCompany信息的方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task Delete(EntityDto<Guid> input)
        {
            //TODO:删除前的逻辑判断，是否允许删除
            await _entityRepository.DeleteAsync(input.Id);
        }



        /// <summary>
        /// 批量删除AbpCompany的方法
        /// </summary>

        public async Task BatchDelete(List<Guid> input)
        {
            // TODO:批量删除前的逻辑判断，是否允许删除
            await _entityRepository.DeleteAsync(s => input.Contains(s.Id));
        }
    }
}


