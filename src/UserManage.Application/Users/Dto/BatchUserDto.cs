using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.Users.Dto
{
    public class BatchUserDto
    {   /// <summary>
        /// 用户id
        /// </summary>
        public long UserId { get; set; }

        public string[] RoleNames { get; set; }
    }
}
