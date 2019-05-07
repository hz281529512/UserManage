

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
using UserManage.AbpDataDictCore;
using Abp.Runtime.Caching;
using Abp.Domain.Uow;

namespace UserManage.AbpDataDictCore.DomainService
{
    /// <summary>
    /// AbpDataDict领域层的业务管理
    ///</summary>
    public class AbpDataDictManager :UserManageDomainServiceBase, IAbpDataDictManager
    {

        private readonly IRepository<AbpDataDict, int> _repository;

        //缓存
        private readonly ICacheManager _cacheManager;

        //UOW
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// AbpDataDict的构造方法
        ///</summary>
        public AbpDataDictManager(
            IRepository<AbpDataDict, int> repository,
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager
        )
		{
            _repository = repository;
            _cacheManager = cacheManager;
            _unitOfWorkManager = unitOfWorkManager;
        }

        // TODO:编写领域业务代码

        private async Task<ICollection<AbpDataDict>> getDataDictWithoutTenant(string itemType)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                return await _repository.GetAll().AsNoTracking().Where(x => x.ItemType == itemType).OrderBy(c => c.ItemSort).ToListAsync();
            }
        }

        /// <summary>
        /// 根据类型 获取所有数据字典
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public Task<ICollection<AbpDataDict>> GetDataDictWithoutTenant(string itemType)
        {
            return _cacheManager.GetCache(UserManageConsts.Abp_Data_Dict_Cache).GetAsync(itemType, () => this.getDataDictWithoutTenant(itemType));
        }


        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <returns></returns>
        public async Task ClearCache()
        {
            await _cacheManager.GetCache(UserManageConsts.Abp_Data_Dict_Cache).ClearAsync();
        }


    }
}
