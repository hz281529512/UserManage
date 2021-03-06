using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using UserManage.Authorization.Roles;
using UserManage.Authorization.Users;
using UserManage.MultiTenancy;
using Abp.IdentityServer4;
using IdentityServer4.Stores;
using UserManage.AbpExternalAuthenticateCore;
using UserManage.AbpOrganizationUnitCore;
using UserManage.AbpTagCore;
using UserManage.AbpCompanyCore;
using UserManage.AbpDataDictCore;
using UserManage.ThirdPartyConfigCore;
using UserManage.AbpServiceCore;
using UserManage.BaseEntityCore;

namespace UserManage.EntityFrameworkCore
{
    public class UserManageDbContext : AbpZeroDbContext<Tenant, Role, User, UserManageDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<PersistedGrantEntity> PersistedGrants { get; set; }

        #region 供应商扩展
        /// <summary>
        /// 公司
        /// </summary>
        public DbSet<AbpCompany> AbpCompany { get; set; }

        #endregion

        #region 系统管理

        /// <summary>
        /// 数据字典 
        /// </summary>
        public DbSet<AbpDataDict> AbpDataDict { get; set; }
        

        #endregion

        #region 外部应用（企业微信|第三方）与租户的关系配置

        /// <summary>
        /// 外部应用与租户的关系配置
        /// </summary>
        public DbSet<AbpExternalAuthenticateConfig> AbpExternalAuthenticateConfig { get; set; }

        /// <summary>
        /// 第三方应用配置
        /// </summary>
        public DbSet<ThirdPartyConfig> ThirdPartyConfig { get; set; }

        /// <summary>
        /// 内部应用配置
        /// </summary>
        public DbSet<AbpServiceConfig> AbpServiceConfig { get; set; }
        

        #endregion

        #region 组织扩展

        public virtual DbSet<AbpOrganizationUnitExtend> AbpOrganizationUnitExtend { get; set; }

        #endregion

        #region 企业微信附属

        /// <summary>
        /// 企业微信 - 标签
        /// </summary>
        public DbSet<AbpTag> AbpTag { get; set; }

        /// <summary>
        /// 企业微信标签 - 本地用户 - 对照
        /// </summary>
        public DbSet<AbpUserTag> AbpUserTag { get; set; }

        #endregion

        #region Base_User

        /// <summary>
        /// Base User Emp
        /// </summary>
        public DbSet<BaseUserEmp> BaseUserEmp { get; set; }

        public DbSet<BaseUserRole> BaseUserRole { get; set; }

        public DbSet<BaseUserEmpRole> BaseUserEmpRole { get; set; }

        public DbSet<BaseUserOrg> BaseUserOrg { get; set; }

        public DbSet<BaseUserEmpOrg> BaseUserEmpOrg { get; set; }
        

        #endregion

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
