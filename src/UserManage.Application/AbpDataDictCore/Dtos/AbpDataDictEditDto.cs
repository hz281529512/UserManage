
using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;
using UserManage.AbpDataDictCore;

namespace  UserManage.AbpDataDictCore.Dtos
{
    public class AbpDataDictEditDto
    {

        /// <summary>
        /// Id
        /// </summary>
        public int? Id { get; set; }



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