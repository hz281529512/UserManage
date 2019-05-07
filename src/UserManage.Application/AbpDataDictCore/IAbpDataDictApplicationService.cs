
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
using Abp.Authorization;
using Abp.Linq.Extensions;
using Abp.Domain.Repositories;
using Abp.Application.Services;
using Abp.Application.Services.Dto;


using UserManage.AbpDataDictCore.Dtos;
using UserManage.AbpDataDictCore;

namespace UserManage.AbpDataDictCore
{
    /// <summary>
    /// AbpDataDict应用层服务的接口方法
    ///</summary>
    public interface IAbpDataDictAppService : IApplicationService
    {
        /// <summary>
		/// 获取AbpDataDict的分页列表信息
		///</summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<AbpDataDictListDto>> GetPaged(GetAbpDataDictsInput input);
        /// <summary>
        /// 数据字典list
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ListResultDto<AbpDataDictListDto>> GetList(string filter);

        /// <summary>
        /// 通过指定id获取AbpDataDictListDto信息
        /// </summary>
        Task<AbpDataDictListDto> GetById(EntityDto<int> input);

        /// <summary>
        /// 获取全部字典类型
        /// </summary>
        /// <returns></returns>
        Task<ListResultDto<string>> GetAllDataType();

        /// <summary>
        /// 根据类型获取指定数据字典缓存列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<ListResultDto<AbpDataDictListDto>> GetListByType(string type);

        /// <summary>
        /// 根据类型获取指定数据字典缓存列表(免租户)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<ListResultDto<AbpDataDictListDto>> GetListByTypeWithNoTenant(string type);


        /// <summary>
        /// 返回实体的EditDto
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<GetAbpDataDictForEditOutput> GetForEdit(NullableIdDto<int> input);


        /// <summary>
        /// 添加或者修改AbpDataDict的公共方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task CreateOrUpdate(CreateOrUpdateAbpDataDictInput input);


        /// <summary>
        /// 删除AbpDataDict信息的方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Delete(EntityDto<int> input);


        /// <summary>
        /// 批量删除AbpDataDict
        /// </summary>
        Task BatchDelete(List<int> input);

    }
}
