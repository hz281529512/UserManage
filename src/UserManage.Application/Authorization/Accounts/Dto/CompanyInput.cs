using System;
using System.Collections.Generic;
using System.Text;
using Abp.AutoMapper;
using UserManage.AbpCompanyCore;

namespace UserManage.Authorization.Accounts.Dto
{
    [AutoMap(typeof(AbpCompany))]
    public class CompanyInput
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
        public string ComPanyTel { get; set; }
    }
}
