using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.SynchronizeCore.Dtos
{
    public class BaseUserEmpDto
    {
        /// <summary>
        /// ABP 用户ID
        /// </summary>
        public long? AbpUserId { get; set; }

        /// <summary>
        /// 原Base_user.cailian_emp 用户Id
        /// </summary>
        public string EmpUserId { get; set; }

        /// <summary>
        /// 原Base_user.cailian_emp Guid
        /// </summary>
        public string EmpUserGuid { get; set; }

        /// <summary>
        /// 是否头目
        /// </summary>
        public string IsLeader { get; set; }

        /// <summary>
        /// 原Base_user.cailian_emp StationId
        /// </summary>
        public string EmpStationId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public string EmpOrderNo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string EmpStatus { get; set; }
    }
}
