using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.AbpOrganizationUnitCore
{
    /// <summary>
    /// 组织表扩展
    /// </summary>
    public class AbpOrganizationUnitExtend : Abp.Organizations.OrganizationUnit
    {

        /// <summary>
        /// 微信部门Id
        /// </summary>
        public virtual int? WXDeptId { get; set; }

        /// <summary>
        /// 微信父级部门Id
        /// </summary>
        public virtual int? WXParentDeptId { get; set; }

        /// <summary>
        /// 企业邮箱部门Id
        /// </summary>
        public virtual long? QYMailDeptId { get; set; }


        

    }
}
