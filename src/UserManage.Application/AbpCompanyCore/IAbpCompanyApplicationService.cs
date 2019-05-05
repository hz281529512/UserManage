
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


using UserManage.AbpCompanyCore.Dtos;
using UserManage.AbpCompanyCore;

namespace UserManage.AbpCompanyCore
{
    /// <summary>
    /// AbpCompany应用层服务的接口方法
    ///</summary>
    public interface IAbpCompanyAppService : IApplicationService
    {
        /// <summary>
		/// 获取AbpCompany的分页列表信息
		///</summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<AbpCompanyListDto>> GetPaged(GetAbpCompanysInput input);


		/// <summary>
		/// 通过指定id获取AbpCompanyListDto信息
		/// </summary>
		Task<AbpCompanyListDto> GetById(EntityDto<Guid> input);


        /// <summary>
        /// 返回实体的EditDto
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<GetAbpCompanyForEditOutput> GetForEdit(NullableIdDto<Guid> input);


        /// <summary>
        /// 添加或者修改AbpCompany的公共方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task CreateOrUpdate(CreateOrUpdateAbpCompanyInput input);


        /// <summary>
        /// 删除AbpCompany信息的方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Delete(EntityDto<Guid> input);


        /// <summary>
        /// 批量删除AbpCompany
        /// </summary>
        Task BatchDelete(List<Guid> input);


		/// <summary>
        /// 导出AbpCompany为excel表
        /// </summary>
        /// <returns></returns>
		//Task<FileDto> GetToExcel();

    }
}
