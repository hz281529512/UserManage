using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UserManage.SynchronizeCore.Dtos
{
    public class ChangePasswordInput
    {
        /// <summary>
        /// Abp UserId
        /// </summary>
        [Required]
        public long AbpUserId { get; set; }

        /// <summary>
        /// 旧密码(必须8位)
        /// </summary>
        [Required]
        public string OldPassword { get; set; }

        /// <summary>
        /// 新密码(必须8位)
        /// </summary>
        [Required]
        public string NewPassword { get; set; }

        /// <summary>
        /// 重复密码
        /// </summary>
        [Required]
        public string RepeatPassword { get; set; }
    }
}
