using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.AbpTagCore
{
    /// <summary>
    /// 标签 
    /// </summary>
    public class AbpTag : FullAuditedEntity<int>, IMayHaveTenant
    {

        /// <summary>
        /// 租户Id
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// 企业微信 标签Id
        /// </summary>
        public int? ExternalTagId { get; set; }

        /// <summary>
        /// 标签名
        /// </summary>
        public string TagName { get; set; }
    }
}
