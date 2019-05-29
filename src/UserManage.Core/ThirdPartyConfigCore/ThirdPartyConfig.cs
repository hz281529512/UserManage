using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.ThirdPartyConfigCore
{
    /// <summary>
    /// 第三方应用配置
    /// </summary>
    public class ThirdPartyConfig : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 第三方应用名称
        /// </summary>
        public string AppName { get; set; }
        
        /// <summary>
        /// 提供服务
        /// </summary>
        public string LoginProvider { get; set; }

        /// <summary>
        /// 第一方应用ID
        /// </summary>
        public string CropId { get; set; }

        /// <summary>
        /// 第三方应用ID
        /// </summary>
        public string SuiteID { get; set; }

        /// <summary>
        /// 应用秘钥
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// EncodingAESKey
        /// </summary>
        public string EncodingAESKey { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
