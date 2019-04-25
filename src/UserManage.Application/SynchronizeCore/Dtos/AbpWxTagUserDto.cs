using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.SynchronizeCore.Dtos
{
    public class AbpWxTagUserDto
    {
        /// <summary>
        /// 企业微信用户Id
        /// </summary>
        public string userid { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Abp用户Id
        /// </summary>
        public long? AbpUserId { get; set; }

        /// <summary>
        /// 用户 权限 标识
        /// </summary>
        public long? UserRoleId { get; set; }

    }
}
