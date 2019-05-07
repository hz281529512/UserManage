using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.Users.Dto
{
    /// <summary>
    /// 根据角色ID 与where 条件(基于User) 查询用户列表
    /// </summary>
    public class GetUserRoleInput : IShouldNormalize
    {

        /// <summary>
        /// 角色ID
        /// </summary>
        public int? RoleId { get; set; }

        /// <summary>
        /// 过滤
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// 排序 例 id倒叙 : u.Id desc
        /// </summary>
        public string Sorting { get; set; }

        /// <summary>
        /// Sorting 初始化 
        /// </summary>
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "u.Id desc";
            }
        }
    }
}
