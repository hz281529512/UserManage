

using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using UserManage.AbpCompanyCore;

namespace UserManage.AbpCompanyCore.Dtos
{
    public class AbpCompanyListDto : FullAuditedEntityDto<Guid> 
    {

        
		/// <summary>
		/// CompanyNo
		/// </summary>
		public string CompanyNo { get; set; }



		/// <summary>
		/// CompanyName
		/// </summary>
		public string CompanyName { get; set; }



		/// <summary>
		/// CompanyType
		/// </summary>
		public string CompanyType { get; set; }



		/// <summary>
		/// CompanyTel
		/// </summary>
		public string CompanyTel { get; set; }



		/// <summary>
		/// CompanyAddress
		/// </summary>
		public string CompanyAddress { get; set; }



		/// <summary>
		/// CompanyMail
		/// </summary>
		public string CompanyMail { get; set; }



		/// <summary>
		/// LegalPerson
		/// </summary>
		public string LegalPerson { get; set; }



		/// <summary>
		/// LegalPersonTel
		/// </summary>
		public string LegalPersonTel { get; set; }



		/// <summary>
		/// Code
		/// </summary>
		public string Code { get; set; }




    }
}