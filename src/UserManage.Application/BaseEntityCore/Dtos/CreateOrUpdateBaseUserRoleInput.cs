

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UserManage.BaseEntityCore;

namespace UserManage.BaseEntityCore.Dtos
{
    public class CreateOrUpdateBaseUserRoleInput
    {
        [Required]
        public BaseUserRoleEditDto BaseUserRole { get; set; }

    }
}