using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.BaseEntityCore
{
    public class BaseUserEmpOrg : FullAuditedEntity<int>
    {

        public string BaseUserGuid { get; set; }

        public long? AbpUserId { get; set; }

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
