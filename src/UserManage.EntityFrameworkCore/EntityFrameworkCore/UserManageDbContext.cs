using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using UserManage.Authorization.Roles;
using UserManage.Authorization.Users;
using UserManage.MultiTenancy;

namespace UserManage.EntityFrameworkCore
{
    public class UserManageDbContext : AbpZeroDbContext<Tenant, Role, User, UserManageDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public UserManageDbContext(DbContextOptions<UserManageDbContext> options)
            : base(options)
        {
        }
    }
}
