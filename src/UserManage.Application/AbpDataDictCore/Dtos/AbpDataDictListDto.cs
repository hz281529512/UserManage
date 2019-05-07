

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
        /// �ֵ���
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// �ֵ��ϼ�key
        /// </summary>
        public int? ItemParent { get; set; }

        /// <summary>
        /// �ֵ�����
        /// </summary>
        public string ItemType { get; set; }



        /// <summary>
        /// �ֵ�����
        /// </summary>
        public int? ItemSort { get; set; }

        /// <summary>
        /// �ֵ���(��չ)
        /// </summary>
        public string ItemCode { get; set; }


        /// <summary>
        /// ����
        /// </summary>
        public string Describe { get; set; }




    }
}