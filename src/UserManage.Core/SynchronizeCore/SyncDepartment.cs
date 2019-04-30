using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.SynchronizeCore
{
    /// <summary>
    /// 企业微信回调 部门
    /// </summary>
    public class SyncDepartment
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public string changetype { get; set; }

        /// <summary>
        /// 部门id
        /// </summary>
        public int? id { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 父节点ID，根部门为1
        /// </summary>
        public int? parentid { get; set; }

        /// <summary>
        /// 在父部门中的次序值。order值小的排序靠前。
        /// </summary>
        public int order { get; set; }
    }
}
