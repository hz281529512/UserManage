using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using Abp.AutoMapper;
using UserManage.Authorization.Roles;

namespace UserManage.Roles.Dto
{
    [AutoMap(typeof(Role))]
    public class RoleDto : EntityDto<int>
    {
        [Required]
        [StringLength(AbpRoleBase.MaxNameLength)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(AbpRoleBase.MaxDisplayNameLength)]
        public string DisplayName { get; set; }

        public string NormalizedName { get; set; }
        
        [StringLength(Role.MaxDescriptionLength)]
        public string Description { get; set; }

        public List<string> Permissions { get; set; }

        /// <summary>
        /// ��ɫ���
        /// </summary>
        public int? RoleType { get; set; }

        /// <summary>
        /// �����Ͳ��ɫ�༭
        /// </summary>
        public bool? AllowLowEdit { get; set; }
    }
}