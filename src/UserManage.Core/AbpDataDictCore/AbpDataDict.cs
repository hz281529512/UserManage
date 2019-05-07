using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.AbpDataDictCore
{
    /// <summary>
    /// 数据字典
    /// </summary>
    public class AbpDataDict : FullAuditedEntity<int>, IMayHaveTenant
    {

        /// <summary>
        /// 租户ID
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// 字典名
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 字典上级key
        /// </summary>
        public int? ItemParent { get; set; }

        /// <summary>
        /// 字典类型
        /// </summary>
        public string ItemType { get; set; }

        /// <summary>
        /// 字典排序
        /// </summary>
        public int? ItemSort { get; set; }

        /// <summary>
        /// 字典编号(扩展)
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; }

    }
}
