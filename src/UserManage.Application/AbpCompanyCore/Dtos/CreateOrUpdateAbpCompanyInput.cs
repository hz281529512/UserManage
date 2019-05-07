

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UserManage.AbpCompanyCore;

namespace UserManage.AbpCompanyCore.Dtos
{
    public class CreateOrUpdateAbpCompanyInput
    {
        [Required]
        public AbpCompanyEditDto AbpCompany { get; set; }

    }
}