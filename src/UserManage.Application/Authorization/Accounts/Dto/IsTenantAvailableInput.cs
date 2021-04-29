using System.ComponentModel.DataAnnotations;
using Abp.MultiTenancy;

namespace UserManage.Authorization.Accounts.Dto
{
    public class IsTenantAvailableInput
    {   
        /// <summary>
        /// 租户名称
        /// </summary>
        [Required]
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        public string TenancyName { get; set; }
    }
}
