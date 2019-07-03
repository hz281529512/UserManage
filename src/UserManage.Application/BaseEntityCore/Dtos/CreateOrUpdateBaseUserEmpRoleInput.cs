

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UserManage.BaseEntityCore;

namespace UserManage.BaseEntityCore.Dtos
{
    public class CreateOrUpdateBaseUserEmpRoleInput
    {
        [Required]
        public BaseUserEmpRoleEditDto BaseUserEmpRole { get; set; }

    }
}