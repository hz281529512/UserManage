

using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using UserManage.AbpDataDictCore;

namespace UserManage.AbpDataDictCore.Dtos
{
    public class AbpDataDictListDto : EntityDto<int>
    {


        /// <summary>
        /// ×ÖµäÃû
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// ×ÖµäÉÏ¼¶key
        /// </summary>
        public int? ItemParent { get; set; }

        /// <summary>
        /// ×ÖµäÀàĞÍ
        /// </summary>
        public string ItemType { get; set; }



        /// <summary>
        /// ×ÖµäÅÅĞò
        /// </summary>
        public int? ItemSort { get; set; }

        /// <summary>
        /// ×Öµä±àºÅ(À©Õ¹)
        /// </summary>
        public string ItemCode { get; set; }


        /// <summary>
        /// ÃèÊö
        /// </summary>
        public string Describe { get; set; }




    }
}