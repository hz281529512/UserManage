using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UserManage.SynchronizeCore.Dtos;

namespace UserManage.SynchronizeCore
{
    /// <summary>
    /// 同步应用层服务的接口方法
    ///</summary>
    public interface ISynchronizeAppService : IApplicationService
    {
        /// <summary>
        /// 同步所有用户与组织关联信息
        /// </summary>
        /// <returns></returns>
        Task<ManyMatchResultDto> MatchDeptAndUser();

        /// <summary>
        /// 同步企业微信 部门 to 组织
        /// </summary>
        /// <returns></returns>
        Task<MatchResultDto> MatchDepartment();


        /// <summary>
        /// 根据企业微信导入用户
        /// </summary>
        /// <returns></returns>
        Task<MatchResultDto> MatchUser();

        /// <summary>
        /// 同步用户部门关联
        /// </summary>
        /// <returns></returns>
        Task<MatchResultDto> MatchDepartmentUsers();

        /// <summary>
        /// 根据企业微信标签导入用户
        /// </summary>
        /// <returns></returns>
        Task<MatchResultDto> MatchTag();
    }
}

