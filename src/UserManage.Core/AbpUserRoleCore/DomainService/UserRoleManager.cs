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

namespace UserManage.AbpUserRoleCore.DomainService
{
    public class UserRoleManager :UserManageDomainServiceBase, IUserRoleManager
    {
        private readonly IRepository<UserRole, long> _repository;

        private readonly IRepository<Role, int> _roleRepository;

        public UserRoleManager(
            IRepository<UserRole, long> repository,
            IRepository<Role, int> roleRepository
         )
        {
            _repository = repository;
            _roleRepository = roleRepository;
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

        public async Task<int?> MaxRoleTypeByUserIdAsync(long? uid)
        {
            if (uid.HasValue)
            {
                var query = from q in _roleRepository.GetAll().AsNoTracking()
                            where _repository.GetAll().Any(x => x.UserId == uid && x.RoleId == q.Id)
                            select q;
                return query.Max(x => x.RoleType ?? 0);
            }
            return null;
        }
    }
}
