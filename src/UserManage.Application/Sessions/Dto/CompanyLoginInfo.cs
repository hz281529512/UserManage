using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using UserManage.AbpCompanyCore;

namespace UserManage.Sessions.Dto
{
    [AutoMapFrom(typeof(AbpCompany))]
    public class CompanyLoginInfo : EntityDto<Guid>
    {



        /// <summary>
        /// 公司信用机构代码
        /// </summary>
        public string CompanyNo { get; set; }


        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }


        /// <summary>
        /// 公司类型
        /// </summary>
        public string CompanyType { get; set; }


        /// <summary>
        /// ComPanyTel
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

        public string LegalPersonTel { get; set; }
    }
}
