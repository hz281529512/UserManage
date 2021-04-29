

using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace  UserManage.Dtos
{
    public class PagedAndFilteredInputDto : IPagedResultRequest
    {   
        /// <summary>
        /// ��󷵻�����
        /// </summary>
        [Range(1, AppLtmConsts.MaxPageSize)]
        public int MaxResultCount { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public string Filter { get; set; }







        public PagedAndFilteredInputDto()
        {
            MaxResultCount = AppLtmConsts.DefaultPageSize;
        }
    }
}