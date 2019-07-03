
using Abp.Runtime.Validation;
using UserManage.Dtos;
using UserManage.BaseEntityCore;

namespace UserManage.BaseEntityCore.Dtos
{
    public class GetBaseUserRolesInput :  IShouldNormalize
    {
        /// <summary>
        /// 用户中心的User Id
        /// </summary>
        public long? AbpUserId { get; set; }

        /// <summary>
        /// 过滤
        /// </summary>
        public string Filter { get; set; }


        public string Sorting { get; set; }

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
