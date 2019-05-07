using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using UserManage.Dtos;

namespace UserManage.Users.Dto
{
    public class GetUserInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        /// <summary>
        /// 正常化排序使用
        /// </summary>
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "u.Id desc";
            }
        }
    }
}
