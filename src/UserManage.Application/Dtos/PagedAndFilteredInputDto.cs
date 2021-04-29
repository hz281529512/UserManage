

using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace  UserManage.Dtos
{
    public class PagedAndFilteredInputDto : IPagedResultRequest
    {   
        /// <summary>
        /// 最大返回数量
        /// </summary>
        [Range(1, AppLtmConsts.MaxPageSize)]
        public int MaxResultCount { get; set; }
        /// <summary>
        /// 跳过数量
        /// </summary>
        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }
        /// <summary>
        /// 过滤条件
        /// </summary>
        public string Filter { get; set; }







        public PagedAndFilteredInputDto()
        {
            MaxResultCount = AppLtmConsts.DefaultPageSize;
        }
    }
}