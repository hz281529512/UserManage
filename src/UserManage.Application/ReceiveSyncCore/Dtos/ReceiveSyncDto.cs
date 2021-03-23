using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.ReceiveSyncCore.Dtos
{
    public class ReceiveSyncDto
    {
        /// <summary>
        /// 操作 : 用户 (create_user | modify_user | delete_user)
        ///         角色 (create_role | modify_role | delete_role)
        ///         部门 (create_dept | modify_dept | delete_dept)
        /// </summary>
        public string ActionName { get; set; }
    }
}
