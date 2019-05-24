

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
using Abp.Domain.Uow;
using Newtonsoft.Json;

namespace UserManage.AbpExternalAuthenticateCore.DomainService
{
    /// <summary>
    /// AbpExternalAuthenticateConfig领域层的业务管理
    ///</summary>
    public class AbpExternalAuthenticateConfigManager : UserManageDomainServiceBase, IAbpExternalAuthenticateConfigManager
    {

        private readonly IRepository<AbpExternalAuthenticateConfig, int> _repository;

        //缓存
        private readonly ICacheManager _cacheManager;

        public IAbpSession AbpSession { get; set; }
        //UOW
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// AbpExternalAuthenticateConfig的构造方法
        ///</summary>
        public AbpExternalAuthenticateConfigManager(
            IRepository<AbpExternalAuthenticateConfig, int> repository,
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager
        )
        {
            _repository = repository;
            _cacheManager = cacheManager;
            _unitOfWorkManager = unitOfWorkManager;
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
            var resultConfig = await _cacheManager.GetCache(providerKey).GetOrDefaultAsync(str_tenant);
            if (resultConfig == null)
            {
                var auth = await _repository.FirstOrDefaultAsync(x => x.LoginProvider == providerKey);
                if (auth == null) return null;
                var str_auth = JsonConvert.SerializeObject(auth);
                await _cacheManager.GetCache(providerKey).SetAsync(str_tenant, str_auth, TimeSpan.FromHours(8));
                return auth;
            }
            else
            {
                return JsonConvert.DeserializeObject<AbpExternalAuthenticateConfig>(resultConfig as string);
            }
        }

        public AbpExternalAuthenticateConfig GetCurrentAuth(string providerKey, int tenant_id)
        {
            string str_tenant = tenant_id.ToString();

            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                //读取缓存
                var resultConfig = _cacheManager.GetCache(providerKey).GetOrDefault(str_tenant);
                if (resultConfig == null)
                {
                    var auth = _repository.FirstOrDefault(x => x.LoginProvider == providerKey);
                    if (auth == null) return null;
                    var str_auth = JsonConvert.SerializeObject(auth);
                    _cacheManager.GetCache(providerKey).Set(str_tenant, str_auth, TimeSpan.FromHours(8));
                    return auth;
                }
                else
                {
                    return JsonConvert.DeserializeObject<AbpExternalAuthenticateConfig>(resultConfig as string);
                }
            }
        }
    }
}
