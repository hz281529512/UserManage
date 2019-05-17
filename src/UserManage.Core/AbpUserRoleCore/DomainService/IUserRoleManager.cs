using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UserManage.AbpUserRoleCore.DomainService
{
    public interface IUserRoleManager : IDomainService
    {
        /// <summary>
        /// 根据用户Id 获取 角色ID列表
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        Task<List<int>> FindRolesByUserIdAsync(long? uid);
    }
}
