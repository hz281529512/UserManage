using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.AbpExternalCore
{
    /// <summary>
    /// 用户部门对照
    /// </summary>
    public class AbpWeChatUserDepartment
    {
        public string userid { get; set; }

        public string name { get; set; }

        public ICollection<int> department { get; set; }
    }
}
