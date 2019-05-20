using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.Users.Dto
{
    public class BatchUserDistrictInputDto
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 地区数组
        /// </summary>
        public string[] Districts { get; set; }
    }
}
