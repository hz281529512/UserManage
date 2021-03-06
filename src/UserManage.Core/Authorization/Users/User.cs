using System;
using System.Collections.Generic;
using Abp.Authorization.Users;
using Abp.Extensions;

namespace UserManage.Authorization.Users
{
    public class User : AbpUser<User>
    {
        /// <summary>
        /// 职务
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public bool Sex { get; set; }

        /// <summary>
        /// 公司id
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// 地区权限过滤
        /// </summary>
        public string SelectDistrict { get; set; }

        /// <summary>
        /// 企业微信头像Url
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 客户限额
        /// </summary>
        public int? ClientQuota { get; set; }

        /// <summary>
        /// 是否同步创建企业邮箱
        /// </summary>
        public bool? IsCreateEmail { get; set; }

        public const string DefaultPassword = "123qwe";

        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        public static User CreateTenantAdminUser(int tenantId, string emailAddress)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = AdminUserName,
                Name = AdminUserName,
                Surname = AdminUserName,
                EmailAddress = emailAddress,
                Roles = new List<UserRole>()
            };

            user.SetNormalizedNames();

            return user;
        }
    }
}
