

using System;
using System.Threading.Tasks;
using Abp;
using Abp.Domain.Services;
using UserManage.AbpServiceCore;


namespace UserManage.AbpServiceCore.DomainService
{
    public interface IAbpServiceConfigManager : IDomainService
    {

        /// <summary>
        /// 获取当前配置
        /// </summary>
        /// <param name="local_name"></param>
        /// <returns></returns>
        Task<AbpServiceConfig> GetCurrentConfigAsync(string local_name);



    }
}
