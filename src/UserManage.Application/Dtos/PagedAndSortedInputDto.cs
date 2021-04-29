

using Abp.Application.Services.Dto;

namespace UserManage.Dtos
{
    public class PagedAndSortedInputDto : PagedInputDto, ISortedResultRequest
    {
        /// <summary>
        /// ÅÅÐò×Ö¶Î
        /// </summary>
        public string Sorting { get; set; }


		 
		 
         

        public PagedAndSortedInputDto()
        {
            MaxResultCount = AppLtmConsts.DefaultPageSize;
        }
    }
}