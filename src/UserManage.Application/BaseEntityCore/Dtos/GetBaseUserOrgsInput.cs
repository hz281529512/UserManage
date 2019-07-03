
using Abp.Runtime.Validation;
using UserManage.Dtos;
using UserManage.BaseEntityCore;

namespace UserManage.BaseEntityCore.Dtos
{
    public class GetBaseUserOrgsInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {

        /// <summary>
		/// OrgGuid
		/// </summary>
		public string OrgGuid { get; set; }

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
