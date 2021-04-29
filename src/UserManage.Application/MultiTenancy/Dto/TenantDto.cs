using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.MultiTenancy;

namespace UserManage.MultiTenancy.Dto
{
    [AutoMapFrom(typeof(Tenant))]
    public class TenantDto : EntityDto
    {   
        /// <summary>
        /// �⻧����
        /// </summary>
        [Required]
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        [RegularExpression(AbpTenantBase.TenancyNameRegex)]
        public string TenancyName { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [Required]
        [StringLength(AbpTenantBase.MaxNameLength)]
        public string Name { get; set; }        
        /// <summary>
        /// �Ƿ񼤻�
        /// </summary>
        public bool IsActive {get; set;}
    }
}
