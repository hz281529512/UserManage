using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.Authorization.Accounts.Dto
{
    public class ResetPasswordInput
    {   
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 重置验证码
        /// </summary>
        public string ResetCode { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}
