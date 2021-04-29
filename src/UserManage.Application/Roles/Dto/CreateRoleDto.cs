using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Roles;
using Abp.AutoMapper;
using UserManage.Authorization.Roles;

namespace UserManage.Roles.Dto
{
    [AutoMapTo(typeof(Role))]
    public class CreateRoleDto
    {   
        /// <summary>
        /// ����
        /// </summary>
        [Required]
        [StringLength(AbpRoleBase.MaxNameLength)]
        public string Name { get; set; }
        /// <summary>
        /// ��ʾ����
        /// </summary>
        [Required]
        [StringLength(AbpRoleBase.MaxDisplayNameLength)]
        public string DisplayName { get; set; }
        /// <summary>
        /// ͳһ����
        /// </summary>
        public string NormalizedName { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [StringLength(Role.MaxDescriptionLength)]
        public string Description { get; set; }
        /// <summary>
        /// Ȩ��
        /// </summary>
        public List<string> Permissions { get; set; }
    }
}
