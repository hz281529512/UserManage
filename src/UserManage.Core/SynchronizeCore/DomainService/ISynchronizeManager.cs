using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UserManage.AbpExternalCore;

namespace UserManage.SynchronizeCore.DomainService
{
    /// <summary>
    /// 同步Manager 接口
    /// </summary>
    public interface ISynchronizeManager : IDomainService
    {
        /// <summary>
        /// 同步单个组织
        /// </summary>
        /// <param name="wx_dept"></param>
        /// <returns>更新的本地Id</returns>
        void MatchSingleDepartment(AbpWeChatDepartment wx_dept, int? tenant_id);


        /// <summary>
        /// 同步单个组织 (无租户验证版本)
        /// </summary>
        /// <param name="wx_dept"></param>
        /// <param name="tenant_id"></param>
        void MatchSingleDepartmentWithoutTenant(AbpWeChatDepartment wx_dept, int? tenant_id);
    }
}
