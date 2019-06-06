

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
using UserManage.AbpServiceCore;
using Abp.Runtime.Caching;
using Newtonsoft.Json;

namespace UserManage.AbpServiceCore.DomainService
{
    /// <summary>
    /// AbpServiceConfig领域层的业务管理
    ///</summary>
    public class AbpServiceConfigManager :UserManageDomainServiceBase, IAbpServiceConfigManager
    {
		
		private readonly IRepository<AbpServiceConfig,Guid> _repository;

        //缓存
        private readonly ICacheManager _cacheManager;

        /// <summary>
        /// AbpServiceConfig的构造方法
        ///</summary>
        public AbpServiceConfigManager(
			IRepository<AbpServiceConfig, Guid> repository,
            ICacheManager cacheManager
        )
		{
			_repository =  repository;
            _cacheManager = cacheManager;
        }


        // TODO:编写领域业务代码

        /// <summary>
        /// 获取当前配置
        /// </summary>
        /// <param name="providerKey"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public async Task<AbpServiceConfig> GetCurrentConfigAsync(string local_name)
        {
            
            //读取缓存
            var resultConfig = await _cacheManager.GetCache(UserManageConsts.Abp_Service_Config_Cache).GetOrDefaultAsync(local_name);
            if (resultConfig == null)
            {
                var auth = await _repository.FirstOrDefaultAsync(x => x.LocalName == local_name);
                if (auth == null) return null;
                var str_auth = JsonConvert.SerializeObject(auth);
                await _cacheManager.GetCache(UserManageConsts.Abp_Service_Config_Cache).SetAsync(local_name, str_auth, TimeSpan.FromHours(8));
                return auth;
            }
            else
            {
                return JsonConvert.DeserializeObject<AbpServiceConfig>(resultConfig as string);
            }
        }





    }
}
