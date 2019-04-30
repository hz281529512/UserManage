

using System;
using System.Threading.Tasks;
using Abp;
using Abp.Domain.Services;
using UserManage.AbpExternalAuthenticateCore;


namespace UserManage.AbpExternalAuthenticateCore.DomainService
{
    public interface IAbpExternalAuthenticateConfigManager : IDomainService
    {
        /// <summary>
        /// 获取当前配置
        /// </summary>
        /// <param name="providerKey"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        Task<AbpExternalAuthenticateConfig> GetCurrentAuth(string providerKey);

        /// <summary>
        /// 获取当前配置 (根据租户)
        /// </summary>
        /// <param name="providerKey"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        AbpExternalAuthenticateConfig GetCurrentAuth(string providerKey,int tenant_id);
    }
}
