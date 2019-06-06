using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.AbpServiceCore
{
    public class AbpServiceConfig : FullAuditedEntity<Guid>
    {

        /// <summary>
        /// 本地名称
        /// </summary>
        public string LocalName { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }
        
        /// <summary>
        /// 提供服务
        /// </summary>
        public string LoginProvider { get; set; }

        /// <summary>
        /// 企业Id
        /// </summary>
        public string CropId { get; set; }

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
