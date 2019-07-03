

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UserManage.BaseEntityCore;

namespace UserManage.BaseEntityCore.Dtos
{
    public class CreateOrUpdateBaseUserOrgInput
    {
        [Required]
        public BaseUserOrgEditDto BaseUserOrg { get; set; }

    }
}