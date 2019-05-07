

using System;
using System.Threading.Tasks;
using Abp;
using Abp.Domain.Services;
using UserManage.AbpCompanyCore;


namespace UserManage.AbpCompanyCore.DomainService
{
    public interface IAbpCompanyManager : IDomainService
    {


        Task<AbpCompany> FindByIdAsync(string id);
        Task<AbpCompany> CreateAsync(AbpCompany companyinput);



    }
}
