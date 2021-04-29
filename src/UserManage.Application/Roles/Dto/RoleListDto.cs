using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace UserManage.Roles.Dto
{
    public class RoleListDto : EntityDto, IHasCreationTime
    {   
        /// <summary>
        ///名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 是否静态
        /// </summary>
        public bool IsStatic { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

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
