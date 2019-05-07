
using Abp.Runtime.Validation;
using UserManage.Dtos;
using UserManage.AbpCompanyCore;

namespace UserManage.AbpCompanyCore.Dtos
{
    public class GetAbpCompanysInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {

        /// <summary>
        /// 正常化排序使用
        /// </summary>
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Id";
            }
        }

    }
}
