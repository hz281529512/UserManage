using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.Users.Dto
{
    public class UserClientQuotaDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 客户限制
        /// </summary>
        public int? ClientQuota { get; set; }
    }
}
