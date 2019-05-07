
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


using UserManage.AbpDataDictCore;
using UserManage.AbpDataDictCore.Dtos;
using UserManage.AbpDataDictCore.DomainService;



namespace UserManage.AbpDataDictCore
{
    /// <summary>
    /// AbpDataDict应用层服务的接口实现方法  
    ///</summary>
    [AbpAuthorize]
    public class AbpDataDictAppService : UserManageAppServiceBase, IAbpDataDictAppService
    {
        private readonly IRepository<AbpDataDict, int> _entityRepository;

        private readonly IAbpDataDictManager _dictManager;

        /// <summary>
        /// 构造函数 
        ///</summary>
        public AbpDataDictAppService(
        IRepository<AbpDataDict, int> entityRepository
        ,IAbpDataDictManager dictManager
        )
        {
            _entityRepository = entityRepository;
            _dictManager = dictManager;
        }


        /// <summary>
        /// 获取AbpDataDict的分页列表信息
        ///</summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task<PagedResultDto<AbpDataDictListDto>> GetPaged(GetAbpDataDictsInput input)
        {
            //_abpDataDictCache["AbpDataDictCaheDto"]
            var query = _entityRepository.GetAll();
            // TODO:根据传入的参数添加过滤条件
            if (!string.IsNullOrEmpty(input.Filter))
            {
                query = query.Where(input.Filter);
            }
            else
            {
                query = query.Where(c => c.ItemParent == null);
            }

            var count = await query.CountAsync();

            var entityList = await query
                    .OrderBy(input.Sorting).AsNoTracking()
                    .PageBy(input)
                    .ToListAsync();

            // var entityListDtos = ObjectMapper.Map<List<AbpDataDictListDto>>(entityList);
            var entityListDtos = entityList.MapTo<List<AbpDataDictListDto>>();

            return new PagedResultDto<AbpDataDictListDto>(count, entityListDtos);
        }
        /// <summary>
        /// 数据字典list
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ListResultDto<AbpDataDictListDto>> GetList(string filter)
        {
            var query = _entityRepository.GetAll();
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(filter);
            }
            else
            {
                query = query.Where(c => c.ItemParent == null);
            }

            var entityList = await query.OrderBy(c => c.ItemSort).ToListAsync();
            var entityListDtos = entityList.MapTo<List<AbpDataDictListDto>>();
            return new ListResultDto<AbpDataDictListDto>(entityListDtos);
        }

        /// <summary>
        /// 获取现时全部字典类型
        /// </summary>
        /// <returns></returns>
        public async Task<ListResultDto<string>> GetAllDataType()
        {
            var query = from q in _entityRepository.GetAll().AsNoTracking()
                        group q by q.ItemType into g
                        select g.Key;
            var result_list = await query.ToListAsync();
            return new ListResultDto<string>(result_list);
        }

        /// <summary>
        /// 根据类型获取指定数据字典缓存列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<ListResultDto<AbpDataDictListDto>> GetListByType(string type)
        {
            var list = await _dictManager.GetDataDictWithoutTenant(type);
            list = list.Where(x => x.TenantId == this.AbpSession.TenantId).OrderBy(c => c.ItemSort).ToList();
            var entityListDtos = list.MapTo<List<AbpDataDictListDto>>();
            return new ListResultDto<AbpDataDictListDto>(entityListDtos);
        }

        /// <summary>
        /// 根据类型获取指定数据字典缓存列表(免租户)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<ListResultDto<AbpDataDictListDto>> GetListByTypeWithNoTenant(string type)
        {
            var list = await _dictManager.GetDataDictWithoutTenant(type);

            var entityListDtos = list.MapTo<List<AbpDataDictListDto>>();
            return new ListResultDto<AbpDataDictListDto>(entityListDtos);
        }


        /// <summary>
        /// 通过指定id获取AbpDataDictListDto信息
        /// </summary>

        public async Task<AbpDataDictListDto> GetById(EntityDto<int> input)
        {


            var entity = await _entityRepository.GetAsync(input.Id);

            return entity.MapTo<AbpDataDictListDto>();
        }

        /// <summary>
        /// 获取编辑 AbpDataDict
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task<GetAbpDataDictForEditOutput> GetForEdit(NullableIdDto<int> input)
        {
            var output = new GetAbpDataDictForEditOutput();
            AbpDataDictEditDto editDto;

            if (input.Id.HasValue)
            {
                var entity = await _entityRepository.GetAsync(input.Id.Value);

                editDto = entity.MapTo<AbpDataDictEditDto>();

                //abpDataDictEditDto = ObjectMapper.Map<List<abpDataDictEditDto>>(entity);
            }
            else
            {
                editDto = new AbpDataDictEditDto();
            }

            output.AbpDataDict = editDto;
            return output;
        }


        /// <summary>
        /// 添加或者修改AbpDataDict的公共方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task CreateOrUpdate(CreateOrUpdateAbpDataDictInput input)
        {

            if (input.AbpDataDict.Id.HasValue)
            {
                await Update(input.AbpDataDict);
            }
            else
            {
                await Create(input.AbpDataDict);
            }
            await _dictManager.ClearCache();
        }


        /// <summary>
        /// 新增AbpDataDict
        /// </summary>

        protected virtual async Task<AbpDataDictEditDto> Create(AbpDataDictEditDto input)
        {
            //TODO:新增前的逻辑判断，是否允许新增

            // var entity = ObjectMapper.Map <AbpDataDict>(input);
            var entity = input.MapTo<AbpDataDict>();
            entity.TenantId = this.AbpSession.TenantId;

            entity = await _entityRepository.InsertAsync(entity);
            return entity.MapTo<AbpDataDictEditDto>();
        }

        /// <summary>
        /// 编辑AbpDataDict
        /// </summary>

        protected virtual async Task Update(AbpDataDictEditDto input)
        {
            var data_count = await _entityRepository.CountAsync(x => x.Id == input.Id);
            //TODO:更新前的逻辑判断，是否允许更新
            if (data_count == 1)
            {
                var entity = await _entityRepository.GetAsync(input.Id.Value);
                input.MapTo(entity);

                // ObjectMapper.Map(input, entity);
                await _entityRepository.UpdateAsync(entity);
            }
            else
            {
                throw new UserFriendlyException("该字典不能被用户修改");
            }
        }



        /// <summary>
        /// 删除AbpDataDict信息的方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task Delete(EntityDto<int> input)
        {
            //TODO:删除前的逻辑判断，是否允许删除
            await _entityRepository.DeleteAsync(input.Id);
        }



        /// <summary>
        /// 批量删除AbpDataDict的方法
        /// </summary>

        public async Task BatchDelete(List<int> input)
        {
            // TODO:批量删除前的逻辑判断，是否允许删除
            await _entityRepository.DeleteAsync(s => input.Contains(s.Id));
        }

    }
}


