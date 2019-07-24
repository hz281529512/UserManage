using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.SynchronizeCore.Dtos
{
    public class BaseUserDeptDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
		/// 部门名称
		/// </summary>
		public string Name { get; set; }

        /// <summary>
		/// 部门Guid
		/// </summary>
		public string OrgGuid { get; set; }

        /// <summary>
        /// 主部门
        /// </summary>
        public bool? IsMaster { get; set; } = false;
    }
}
