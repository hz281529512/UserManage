using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.BaseEntityCore.Dtos
{
    public class BaseUserEmpOrgDto
    {
        public int? Id { get; set; }

        public string BaseUserGuid { get; set; }

        public long? AbpUserId { get; set; }

        /// <summary>
        /// Abp用户名称
        /// </summary>
        public string AbpUserName { get; set; }

        /// <summary>
        /// 原Base_user.cailian_emp 用户Id
        /// </summary>
        public string EmpUserId { get; set; }

        public string EmpUserGuid { get; set; }

        public string DepartmentId { get; set; }

        public string DepartmentGuid { get; set; }

        public string IsMaster { get; set; }

        public string CropId { get; set; }
    }
}
