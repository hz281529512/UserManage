using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.SynchronizeCore.Dtos
{
    public class AbpWxDepartmentsDto
    {
        /// <summary>
        /// 企业微信部门id
        /// </summary>
        public int? wx_id { get; set; }

        /// <summary>
        /// 企业微信 部门名称
        /// </summary>
        public string wx_name { get; set; }

        /// <summary>
        /// 企业微信 父节点ID，根部门为1
        /// </summary>
        public int? wx_parentid { get; set; }

        /// <summary>
        /// Abp 组织Id
        /// </summary>
        public long? abp_id { get; set; }
    }
}
