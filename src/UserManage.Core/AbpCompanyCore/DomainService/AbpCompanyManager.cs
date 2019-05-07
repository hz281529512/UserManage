

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Linq;
using Abp.Linq.Extensions;
using Abp.Extensions;
using Abp.UI;
using Abp.Domain.Repositories;
using Abp.Domain.Services;

using UserManage;
using UserManage.AbpCompanyCore;


namespace UserManage.AbpCompanyCore.DomainService
{
    /// <summary>
    /// AbpCompany领域层的业务管理
    ///</summary>
    public class AbpCompanyManager :UserManageDomainServiceBase, IAbpCompanyManager
    {
		
		private readonly IRepository<AbpCompany,Guid> _repository;

		/// <summary>
		/// AbpCompany的构造方法
		///</summary>
		public AbpCompanyManager(
			IRepository<AbpCompany, Guid> repository
		)
		{
			_repository =  repository;
		}

        // TODO:编写领域业务代码

        public async Task<AbpCompany> FindByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            var data = await _repository.GetAsync(new Guid(id));
            return data;
        }


        public async Task<AbpCompany> CreateAsync(AbpCompany companyinput)
        {
            if (companyinput == null)
                return null;
            var companyData = _repository.FirstOrDefault(c => c.CompanyNo == companyinput.CompanyNo);
            if (companyData != null)
            {
                return companyData;
            }
            else
            {
                companyinput.Code = Guid.NewGuid().ToString().Replace("-", "").ToUpper().Substring(0, 12);
                var data = await _repository.InsertAsync(companyinput);
                return data;
            }
        }





    }
}
