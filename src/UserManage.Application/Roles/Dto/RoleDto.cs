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
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [StringLength(AbpRoleBase.MaxNameLength)]
        public string Name { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        [Required]
        [StringLength(AbpRoleBase.MaxDisplayNameLength)]
        public string DisplayName { get; set; }
        /// <summary>
        /// 统一名称
        /// </summary>
        public string NormalizedName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(Role.MaxDescriptionLength)]
        public string Description { get; set; }
        /// <summary>
        /// 权限
        /// </summary>
        public List<string> Permissions { get; set; }

        /// <summary>
        /// 角色组别
        /// </summary>
        public int? RoleType { get; set; }

        /// <summary>
        /// 允许低层角色编辑
        /// </summary>
        public bool? AllowLowEdit { get; set; }
    }
}