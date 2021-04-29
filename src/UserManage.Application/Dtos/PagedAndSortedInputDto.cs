

using Abp.Application.Services.Dto;

namespace UserManage.Dtos
{
    public class PagedAndSortedInputDto : PagedInputDto, ISortedResultRequest
    {
        /// <summary>
        /// �����ֶ�
        /// </summary>
        public string Sorting { get; set; }


		 
		 
         

        public PagedAndSortedInputDto()
        {
            MaxResultCount = AppLtmConsts.DefaultPageSize;
        }
    }
}