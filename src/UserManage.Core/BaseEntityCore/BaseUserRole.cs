using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.BaseEntityCore
{
    public class BaseUserRole :  FullAuditedEntity<int>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 允许低层角色编辑
        /// </summary>
        public bool? AllowLowEdit { get; set; }
    }
}
