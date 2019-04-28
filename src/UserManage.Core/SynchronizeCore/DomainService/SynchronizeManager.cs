﻿using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManage.AbpExternalCore;
using UserManage.AbpOrganizationUnitCore;
using UserManage.Authorization.Roles;
using UserManage.Authorization.Users;
using UserManage.SynchronizeCore.Dto;

namespace UserManage.SynchronizeCore.DomainService
{
    /// <summary>
    /// 同步 领域层的业务管理
    ///</summary>
    public class SynchronizeManager : UserManageDomainServiceBase, ISynchronizeManager
    {
        
        //组织
        private readonly IRepository<AbpOrganizationUnitExtend, long> _organizationUnitRepository;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;

        //用户
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<UserLogin, long> _userLoginRepository;
        private readonly IPasswordHasher<User> _passwordHasher;


        ////角色
        private readonly IRepository<Role, int> _roleRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;

        //Session
        public IAbpSession AbpSession { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SynchronizeManager(
            IRepository<AbpOrganizationUnitExtend, long> organizationUnitRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IRepository<User, long> userRepository,
            IRepository<UserLogin, long> userLoginRepository,
            IRepository<Role, int> roleRepository,
            IRepository<UserRole, long> userRoleRepository,
            IPasswordHasher<User> passwordHasher
         )
        {
            _organizationUnitRepository = organizationUnitRepository;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _userRepository = userRepository;
            _userLoginRepository = userLoginRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _passwordHasher = passwordHasher;
            AbpSession = NullAbpSession.Instance;
        }

        #region  Synchronize Department

        public async Task<SyncResultDto> MatchDepartments(ICollection<AbpWeChatDepartment> wx_departments)
        {
            // 尝试获取企业微信部门与本地组织full join
            var wx_abp_depts = this.GetWxAbpDepartments(wx_departments);
            return null;
        }

        #region Private

        private async Task<SyncResultDto> SyncWxDepartment(SyncWxDepartmentsDto item)
        {
            var result = new SyncResultDto();
            if (item.abp_id != 0)
            {
                //判断是否拥有企业微信记录,否则删除本地记录
                if (item.wx_id != 0)
                {
                    var entity = _organizationUnitRepository.Get(item.abp_id.Value);
                    entity.WXDeptId = item.wx_id;
                    entity.WXParentDeptId = item.wx_parentid;
                    await _organizationUnitRepository.UpdateAsync(entity);
                }
                else
                {
                    await _organizationUnitRepository.DeleteAsync(item.abp_id.Value);
                    result.DeleteCount++;
                }
            }
            else
            {
                //添加组织记录
                await _organizationUnitRepository.InsertAsync(new AbpOrganizationUnitExtend
                {
                    TenantId = AbpSession.TenantId,
                    WXDeptId = item.wx_id,
                    DisplayName = item.wx_name,
                    WXParentDeptId = item.wx_parentid,
                    Code = ""
                });
                result.CreateCount++;
            }
            return result;
        }

        // <summary>
        /// 再次尝试获取企业微信部门与本地组织full join
        /// </summary>
        /// <param name="wx_dept"></param>
        /// <returns></returns>
        private List<SyncWxDepartmentsDto> GetWxAbpDepartments(ICollection<AbpWeChatDepartment> wx_dept)
        {

            //var o_query = _organizationUnitRepository.GetAll().ToList();
            var left_query = from o in _organizationUnitRepository.GetAll()
                             join dm in wx_dept.AsQueryable() on o.WXDeptId equals dm.id into dori
                             from d in dori.DefaultIfEmpty()
                             where o.TenantId == AbpSession.TenantId
                             select new SyncWxDepartmentsDto
                             {
                                 abp_id = o.Id,
                                 wx_id = (d == null ? 0 : d.id),
                                 wx_name = (d == null ? "" : d.name),
                                 wx_parentid = (d == null ? 0 : d.parentid),
                             };

            var right_query = from d in wx_dept.AsQueryable()
                              where !_organizationUnitRepository.GetAll().Any(o => o.WXDeptId == d.id && o.TenantId == AbpSession.TenantId)
                              select new SyncWxDepartmentsDto
                              {
                                  abp_id = 0,
                                  wx_id = d.id,
                                  wx_name = d.name,
                                  wx_parentid = d.parentid,
                              };
            //var tt = left_query.ToList();
            //var rr = right_query.ToList();
            if (left_query.Any())
            {
                var full_query = left_query.Union(right_query);
                return full_query.ToList();
            }
            else if (right_query.Any())
            {
                return right_query.ToList();
            }
            return null;
        }

        #endregion

        #endregion
    }
}
