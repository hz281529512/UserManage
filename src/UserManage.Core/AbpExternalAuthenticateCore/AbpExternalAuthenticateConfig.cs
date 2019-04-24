using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.AbpExternalAuthenticateCore
{
    /// <summary>
    /// 外部应用与租户的关系配置
    /// </summary>
    public class AbpExternalAuthenticateConfig : FullAuditedEntity<int>, IMayHaveTenant
    {
        /// <summary>
        /// 租户ID
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }


        /// <summary>
        /// 提供服务
        /// </summary>
        public string LoginProvider { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 应用秘钥
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
