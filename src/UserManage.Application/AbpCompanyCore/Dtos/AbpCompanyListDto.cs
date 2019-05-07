

using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using UserManage.AbpCompanyCore;

namespace UserManage.AbpCompanyCore.Dtos
{
    public class AbpCompanyListDto : EntityDto<Guid> 
    {


        /// <summary>
        /// ���û�������
        /// </summary>
        public string CompanyNo { get; set; }
        /// <summary>
        /// ��˾����
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// ��˾����
        /// </summary>
        public string CompanyType { get; set; }
        /// <summary>
        /// ��ϵ�绰
        /// </summary>
        public string CompanyTel { get; set; }
        /// <summary>
        /// ��ַ
        /// </summary>
        public string CompanyAddress { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string CompanyMail { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string LegalPerson { get; set; }
        /// <summary>
        /// ���˵绰
        /// </summary>
        public string LegalPersonTel { get; set; }


    }
}