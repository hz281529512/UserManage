using System;
using System.Collections.Generic;
using System.Text;
using UserManage.AbpExternalCore.Model;

namespace UserManage.QYEmail
{
    internal class QYMailDepartmentResult  : AbpWechatResult
    {
        public List<QYMailDepartment> department { get; set; }
    }

    public class QYMailDepartment
    {
        /// <summary>
        /// 部门id
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 父节点ID，根部门为1
        /// </summary>
        public long parentid { get; set; }

        /// <summary>
        /// 在父部门中的次序值。order值小的排序靠前。
        /// </summary>
        public int order { get; set; }
    }
}
