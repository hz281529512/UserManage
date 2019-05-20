using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using UserManage.Authorization.Roles;
using UserManage.Authorization.Users;

namespace UserManage.AbpUserRoleCore.DomainService
{
    public class UserRoleManager :UserManageDomainServiceBase, IUserRoleManager
    {
        private readonly IRepository<UserRole, long> _repository;

        public RoleManager RoleManager { get; set; }

        public UserRoleManager(
            IRepository<UserRole, long> repository
            
         )
        {
            _repository = repository;
            
        }

        /// <summary>
        /// 根据用户Id 获取 角色ID列表
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<List<int>> FindRolesByUserIdAsync(long? uid)
        {
            if (uid.HasValue)
            {
                var query = from q in _repository.GetAll().AsNoTracking()
                            where q.UserId == uid
                            select q.RoleId;
                return await query.ToListAsync();
            }
            return null;
        }

        public async Task<int?> MaxRoleType(IList<string> roles)
        {
            if (roles?.Count > 0)
            {
                List<Role> result = new List<Role>();
                foreach (var item in roles)
                {
                    result.Add(await RoleManager.GetRoleByNameAsync(item));
                }
                return result.Max(x => x.RoleType ?? 0);
            }
            return 0;
        }
    }
}
