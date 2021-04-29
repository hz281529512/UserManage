

using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace UserManage.Dtos
{
    public class PagedInputDto : IPagedResultRequest
    {   
        /// <summary>
        /// �����������
        /// </summary>
        [Range(1, AppLtmConsts.MaxPageSize)]
        public int MaxResultCount { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }



		 
		 
         


        public PagedInputDto()
        {
            MaxResultCount = AppLtmConsts.DefaultPageSize;
        }
    }
}