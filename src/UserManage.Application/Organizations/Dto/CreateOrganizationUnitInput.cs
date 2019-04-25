using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.AutoMapper;
using Abp.Organizations;

namespace UserManage.Organizations.Dto
{
    [AutoMapTo(typeof(OrganizationUnit))]
    public class CreateOrganizationUnitInput
    {
        /// <summary>
        /// 父节点
        /// </summary>
        public long? ParentId { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        [Required]
        [StringLength(OrganizationUnit.MaxDisplayNameLength)]
        public string DisplayName { get; set; }
    }
}
