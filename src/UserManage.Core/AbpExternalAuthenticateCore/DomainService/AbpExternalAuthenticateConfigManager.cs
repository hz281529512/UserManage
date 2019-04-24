

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
using UserManage.AbpExternalAuthenticateCore;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;

namespace UserManage.AbpExternalAuthenticateCore.DomainService
{
    /// <summary>
    /// AbpExternalAuthenticateConfig领域层的业务管理
    ///</summary>
    public class AbpExternalAuthenticateConfigManager :UserManageDomainServiceBase, IAbpExternalAuthenticateConfigManager
    {
		
		private readonly IRepository<AbpExternalAuthenticateConfig,int> _repository;

        //缓存
        private readonly ICacheManager _cacheManager;

        public IAbpSession AbpSession { get; set; }

        /// <summary>
        /// AbpExternalAuthenticateConfig的构造方法
        ///</summary>
        public AbpExternalAuthenticateConfigManager(
			IRepository<AbpExternalAuthenticateConfig, int> repository,
            ICacheManager cacheManager
        )
		{
			_repository =  repository;
            _cacheManager = cacheManager;
            AbpSession = NullAbpSession.Instance;
        }


        // TODO:编写领域业务代码

        /// <summary>
        /// 获取当前配置
        /// </summary>
        /// <param name="providerKey"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public async Task<AbpExternalAuthenticateConfig> GetCurrentAuth(string providerKey)
        {
            if (!AbpSession.TenantId.HasValue) throw new InvalidOperationException("Can not register host users!");
            string str_tenant = AbpSession.TenantId.ToString();

            //读取缓存
            var resultConfig = await _cacheManager.GetCache(providerKey).GetOrDefaultAsync(str_tenant) as AbpExternalAuthenticateConfig;
            if (resultConfig == null)
            {
                var auth = await _repository.FirstOrDefaultAsync(x => x.LoginProvider == providerKey);
                if (auth == null) return null;
                await _cacheManager.GetCache(providerKey).SetAsync(str_tenant, auth, TimeSpan.FromHours(8));
                return auth;
            }
            else
            {
                return resultConfig;
            }
        }



    }
}
