
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


using UserManage.BaseEntityCore.Dtos;
using UserManage.BaseEntityCore;

namespace UserManage.BaseEntityCore
{
    /// <summary>
    /// BaseUserEmpRole应用层服务的接口方法
    ///</summary>
    public interface IBaseUserEmpRoleAppService : IApplicationService
    {
        /// <summary>
		/// 获取BaseUserEmpRole的分页列表信息
		///</summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<BaseUserEmpRoleListDto>> GetPaged(GetBaseUserEmpRolesInput input);


		/// <summary>
		/// 通过指定id获取BaseUserEmpRoleListDto信息
		/// </summary>
		Task<BaseUserEmpRoleListDto> GetById(EntityDto<int> input);


        /// <summary>
        /// 返回实体的EditDto
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<GetBaseUserEmpRoleForEditOutput> GetForEdit(NullableIdDto<int> input);


        /// <summary>
        /// 添加或者修改BaseUserEmpRole的公共方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<BaseUserRoleEditDto> CreateOrUpdate(CreateOrUpdateBaseUserRoleInput input);


        /// <summary>
        /// 删除BaseUserEmpRole信息的方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task DeleteRole(EntityDto<int> input);


        /// <summary>
        /// 批量删除BaseUserEmpRole
        /// </summary>
        Task BatchDelete(List<int> input);


		/// <summary>
        /// 导出BaseUserEmpRole为excel表
        /// </summary>
        /// <returns></returns>
		//Task<FileDto> GetToExcel();

    }
}
