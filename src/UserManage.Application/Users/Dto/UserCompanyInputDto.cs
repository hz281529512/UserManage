using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.Users.Dto
{
    /// <summary>
    /// 修改公司名DTO
    /// </summary>
    public class UserCompanyInputDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 公司名
        /// </summary>
        public string CompanyName { get; set; }
    }
}
