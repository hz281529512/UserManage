using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using UserManage.Authorization.Roles;
using UserManage.Authorization.Users;
using UserManage.MultiTenancy;
using Abp.IdentityServer4;
using IdentityServer4.Stores;

namespace UserManage.EntityFrameworkCore
{
    public class UserManageDbContext : AbpZeroDbContext<Tenant, Role, User, UserManageDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<PersistedGrantEntity> PersistedGrants { get; set; }
        public UserManageDbContext(DbContextOptions<UserManageDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ConfigurePersistedGrantEntity();
        }
    }
}
