

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp;
using Abp.Domain.Services;
using UserManage.BaseEntityCore;


namespace UserManage.BaseEntityCore.DomainService
{
    public interface IBaseUserEmpManager : IDomainService
    {
        /// <summary>
        /// 根据Abp用户ID获取BaseUserRole列表
        /// </summary>
        /// <param name="AbpUserId"></param>
        /// <returns></returns>
        Task<List<BaseUserRole>> GetBaseUserRoleByAbpId(long AbpUserId);


        /// <summary>
        /// 根据Abp用户ID获取BaseUserOrg列表
        /// </summary>
        /// <param name="AbpUserId"></param>
        /// <returns></returns>
        Task<List<BaseUserOrg>> GetBaseUserOrgByAbpId(long AbpUserId);

        /// <summary>
        /// 根据Abp用户ID获取主部门Id
        /// </summary>
        /// <param name="AbpUserId"></param>
        /// <returns></returns>
        Task<int> GetMasterIdByAbpId(long AbpUserId);

        /// <summary>
        /// 根据微信部门Id获取BaseUserOrg
        /// </summary>
        /// <param name="AbpUserId"></param>
        /// <returns></returns>
        Task<BaseUserOrg> GetBaseUserOrgByWxId(int wx_id);
    }
}
