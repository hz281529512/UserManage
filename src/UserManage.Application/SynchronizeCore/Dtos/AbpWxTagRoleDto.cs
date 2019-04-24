using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.SynchronizeCore.Dtos
{
   public class AbpWxTagRoleDto
    {
        /// <summary>
        /// 微信标签 Id
        /// </summary>
        public int? WechatTagId { get; set; }

        /// <summary>
        /// 标签名
        /// </summary>
        public string TagName { get; set; }

        ///// <summary>
        ///// 微信标签名
        ///// </summary>
        //public string WeChatTagName { get; set; }

        /// <summary>
        /// 角色Id
        /// </summary>
        public int? RoleId { get; set; }
    }
}
