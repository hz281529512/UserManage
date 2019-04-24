using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.AbpExternalCore.Model
{
    internal class AbpWechatResult
    {
        /// <summary>
        /// 错误代码 
        /// </summary>
        public string errcode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string errmsg { get; set; }
    }
}
