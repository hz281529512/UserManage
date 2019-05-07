

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UserManage.AbpDataDictCore;

namespace UserManage.AbpDataDictCore.Dtos
{
    public class CreateOrUpdateAbpDataDictInput
    {
        [Required]
        public AbpDataDictEditDto AbpDataDict { get; set; }

    }
}