using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Roles;
using UserManage.Authorization.Users;

namespace UserManage.Authorization.Roles
{
    public class Role : AbpRole<User>
    {
        public const int MaxDescriptionLength = 5000;

        public Role()
        {
        }

        public Role(int? tenantId, string displayName)
            : base(tenantId, displayName)
        {
        }

        public Role(int? tenantId, string name, string displayName)
            : base(tenantId, name, displayName)
        {
        }

        [StringLength(MaxDescriptionLength)]
        public string Description {get; set;}

        /// <summary>
        /// 微信 标签 Id
        /// </summary>
        public string WxTagId { get; set; }

        /// <summary>
        /// 角色组别
        /// </summary>
        public int? RoleType { get; set; }

    }
}
