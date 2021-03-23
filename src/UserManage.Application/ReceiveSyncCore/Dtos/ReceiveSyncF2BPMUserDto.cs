using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UserManage.ReceiveSyncCore.Dtos
{
    public class ReceiveSyncF2BPMUserDto
    {

        /// <summary>
        /// 企业微信ID
        /// </summary>
        [Required]
        public string WXId { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public ReceiveSyncBaseEmpDto Data { get; set; }
    }

    public class ReceiveSyncBaseEmpDto
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

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
