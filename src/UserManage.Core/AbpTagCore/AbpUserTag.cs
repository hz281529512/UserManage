using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.AbpTagCore
{
    /// <summary>
    /// 用户标签[暂时废弃该方案]
    /// </summary>
    public class AbpUserTag : CreationAuditedEntity<int>, IMayHaveTenant
    {
        /// <summary>
        /// 租户Id
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// 标签Id
        /// </summary>
        public int? TagId { get; set; }

        /// <summary>
        /// 企业微信用户Id
        /// </summary>
        public string QYUserKey { get; set; }
    }
}
