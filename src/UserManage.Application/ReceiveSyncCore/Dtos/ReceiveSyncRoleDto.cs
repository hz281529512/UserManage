using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Extensions;

namespace UserManage.ReceiveSyncCore.Dtos
{
    public class ReceiveSyncRoleDto : ReceiveSyncDto, IValidatableObject
    {
        /// <summary>
        /// 微信标签 Id
        /// </summary>
        [Required]
        public int? WxTagId { get; set; }

        /// <summary>
        /// 权限名
        /// </summary>
        public string RoleName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.ActionName != "delete_role")
            {
                if (this.RoleName.IsNullOrEmpty())
                {
                    yield return new ValidationResult("权限名不能为空!");
                }
            }
        }
    }
}
