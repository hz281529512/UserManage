﻿using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.AbpCompanyCore
{
    public class AbpCompany : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 信用机构代码
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
        /// 联系电话
        /// </summary>
        public string CompanyTel { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string CompanyAddress { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string CompanyMail { get; set; }
        /// <summary>
        /// 法人
        /// </summary>
        public string LegalPerson { get; set; }
        /// <summary>
        /// 法人电话
        /// </summary>
        public string LegalPersonTel { get; set; }

        public string Code { get; set; }

    }
}
