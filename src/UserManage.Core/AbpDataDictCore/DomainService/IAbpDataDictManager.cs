

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp;
using Abp.Domain.Services;
using UserManage.AbpDataDictCore;


namespace UserManage.AbpDataDictCore.DomainService
{
    public interface IAbpDataDictManager : IDomainService
    {

        /// <summary>
        /// 根据类型 获取所有数据字典
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        Task<ICollection<AbpDataDict>> GetDataDictWithoutTenant(string itemType);

        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <returns></returns>
        Task ClearCache();


    }
}
