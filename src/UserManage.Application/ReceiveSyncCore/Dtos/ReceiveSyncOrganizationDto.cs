using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Extensions;

namespace UserManage.ReceiveSyncCore.Dtos
{
    public class ReceiveSyncOrganizationDto : ReceiveSyncDto, IValidatableObject
    {
        /// <summary>
        /// 微信部门Id
        /// </summary>
        [Required]
        public  int? WXDeptId { get; set; }

        /// <summary>
        /// 微信父级部门Id
        /// </summary>
        
        public  int? WXParentDeptId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string DisplayName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.ActionName != "delete_dept")
            {
                if (this.DisplayName.IsNullOrEmpty())
                {
                    yield return new ValidationResult("角色名不能为空!");
                }
                if (!this.WXParentDeptId.HasValue)
                {
                    yield return new ValidationResult("父级ID不能为空!");
                }
            }
        }
    }
}
