

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
using UserManage.BaseEntityCore;


namespace UserManage.BaseEntityCore.DomainService
{
    /// <summary>
    /// BaseUserEmp领域层的业务管理
    ///</summary>
    public class BaseUserEmpManager :UserManageDomainServiceBase, IBaseUserEmpManager
    {
		
		private readonly IRepository<BaseUserEmp,long> _repository;

        private readonly IRepository<BaseUserEmpRole, int> _empRoleRepository;

        private readonly IRepository<BaseUserRole, int> _roleRepository;

        private readonly IRepository<BaseUserOrg, int> _orgRepository;

        private readonly IRepository<BaseUserEmpOrg, int> _empOrgRepository;

        /// <summary>
        /// BaseUserEmp的构造方法
        ///</summary>
        public BaseUserEmpManager(
			IRepository<BaseUserEmp, long> repository,
            IRepository<BaseUserEmpRole, int> empRoleRepository,
            IRepository<BaseUserRole, int> roleRepository,
            IRepository<BaseUserOrg, int> orgRepository,
            IRepository<BaseUserEmpOrg, int> empOrgRepository
        )
		{
			_repository =  repository;
            _empRoleRepository = empRoleRepository;
            _roleRepository = roleRepository;
            _orgRepository = orgRepository;
            _empOrgRepository = empOrgRepository;
        }



        // TODO:编写领域业务代码


        /// <summary>
        /// 根据Abp用户ID获取BaseUserRole列表
        /// </summary>
        /// <param name="AbpUserId"></param>
        /// <returns></returns>
        public async Task<List<BaseUserRole>> GetBaseUserRoleByAbpId(long AbpUserId)
        {
            var str_id = AbpUserId.ToString();
            var query = _roleRepository.GetAll().Where(x => _empRoleRepository.GetAll().Any(y => x.Id == y.BaseRoleId && y.AbpUserId == str_id));
            return await query.ToListAsync();
        }


        /// <summary>
        /// 根据Abp用户ID获取BaseUserOrg列表
        /// </summary>
        /// <param name="AbpUserId"></param>
        /// <returns></returns>
        public async Task<List<BaseUserOrg>> GetBaseUserOrgByAbpId(long AbpUserId)
        {
            var query = _orgRepository.GetAll().Where(x => _empOrgRepository.GetAll().Any(y => x.Id.ToString() == y.DepartmentId && y.AbpUserId == AbpUserId));
            return await query.ToListAsync();
        }

        /// <summary>
        /// 根据微信部门Id获取BaseUserOrg
        /// </summary>
        /// <param name="AbpUserId"></param>
        /// <returns></returns>
        public async Task<BaseUserOrg> GetBaseUserOrgByWxId(int wx_id)
        {
            var str_id = wx_id.ToString();
            return await _orgRepository.FirstOrDefaultAsync(x => x.WxId == str_id);
        }

        /// <summary>
        /// 根据Abp用户ID获取主部门Id
        /// </summary>
        /// <param name="AbpUserId"></param>
        /// <returns></returns>
        public async Task<int> GetMasterIdByAbpId(long AbpUserId)
        {
            var master_org = await _empOrgRepository.FirstOrDefaultAsync(x => x.AbpUserId == AbpUserId && x.IsMaster == "1");
            if (master_org != null)
            {
                return master_org.Id;
            }
            return 0;
        }

    }
}
