
using Abp.Runtime.Validation;
using UserManage.Dtos;
using UserManage.AbpDataDictCore;

namespace UserManage.AbpDataDictCore.Dtos
{
    public class GetAbpDataDictsInput : PagedSortedAndFilteredInputDto, IShouldNormalize
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
