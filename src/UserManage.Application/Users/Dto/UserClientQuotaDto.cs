using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UserManage.Users.Dto
{
    public class UserClientQuotaDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Required]
        public long UserId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 客户限制
        /// </summary>
        public int? ClientQuota { get; set; }
    }
}
