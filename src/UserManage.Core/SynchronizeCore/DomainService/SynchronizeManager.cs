using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Organizations;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManage.AbpExternalCore;
using UserManage.AbpExternalCore.DomainService;
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
        private readonly OrganizationUnitManager _organizationUnitManager;

        //用户
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<UserLogin, long> _userLoginRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly UserManager _userManager;


        ////角色
        private readonly IRepository<Role, int> _roleRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;

        //Session
        public IAbpSession AbpSession { get; set; }

        //UOW
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        //log
        public ILogger _logger;

        //wechat
        private readonly IAbpWeChatManager _weChatManager;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SynchronizeManager(
            IRepository<AbpOrganizationUnitExtend, long> organizationUnitRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            OrganizationUnitManager organizationUnitManager,
            IRepository<User, long> userRepository,
            IRepository<UserLogin, long> userLoginRepository,
            IRepository<Role, int> roleRepository,
            IRepository<UserRole, long> userRoleRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IAbpWeChatManager weChatManager,
            IPasswordHasher<User> passwordHasher
         )
        {
            _organizationUnitRepository = organizationUnitRepository;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _organizationUnitManager = organizationUnitManager;
            _userRepository = userRepository;
            _userLoginRepository = userLoginRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _passwordHasher = passwordHasher;
            _unitOfWorkManager = unitOfWorkManager;
            AbpSession = NullAbpSession.Instance;
            _logger = NullLogger.Instance;
            _weChatManager = weChatManager;
        }

        #region  Synchronize Department

        /// <summary>
        /// 同步单个组织 (无租户验证版本)         
        /// </summary>
        /// <param name="wx_dept"></param>
        /// <returns>更新的本地Id</returns>
        public void MatchSingleDepartmentWithoutTenant(SyncDepartment wx_dept, int? tenant_id)
        {
            if (wx_dept != null)
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    var entity = _organizationUnitRepository.FirstOrDefault(o => o.WXDeptId == wx_dept.id && o.TenantId == tenant_id);
                    var result_id = entity?.Id ?? null;
                    var parent_entity = wx_dept.parentid == 0 ? null : (_organizationUnitRepository.FirstOrDefault(o => o.WXDeptId == wx_dept.parentid && o.TenantId == tenant_id));
                    var parent_id = parent_entity?.Id ?? null;
                    var parent_code = parent_entity?.Code ?? "";

                    switch (wx_dept.changetype)
                    {
                        case "delete_party":
                            if (result_id.HasValue)
                                _organizationUnitRepository.Delete(result_id.Value);
                            break;
                        case "create_party":
                            if (entity == null)
                            {
                                entity = new AbpOrganizationUnitExtend
                                {
                                    TenantId = tenant_id,//AbpSession.TenantId,
                                    WXDeptId = wx_dept.id,
                                    DisplayName = wx_dept.name,
                                    WXParentDeptId = wx_dept.parentid,
                                    ParentId = parent_id,
                                    Code = ""
                                };
                                result_id = _organizationUnitRepository.InsertAndGetId(entity);
                                //CurrentUnitOfWork.SaveChanges();
                                entity.Code = string.IsNullOrEmpty(parent_code) ? result_id.ToString() : parent_code + ":" + result_id.Value;
                                entity.Id = result_id.Value;
                                _organizationUnitRepository.Update(entity);
                            }
                            break;
                        case "update_party":
                            if (result_id.HasValue)
                            {
                                /**
                                 * 2019-04-28 miansheng.luo 企业微信回调有更新忽略。修改时请注意修改下列字段判断方式
                                 */
                                if (wx_dept.parentid.HasValue)
                                {
                                    entity.ParentId = parent_id;
                                    entity.Code = string.IsNullOrEmpty(parent_code) ? result_id.ToString() : parent_code + ":" + result_id.Value;
                                    entity.WXParentDeptId = wx_dept.parentid;
                                }
                                entity.TenantId = tenant_id;
                                entity.WXDeptId = wx_dept.id;
                                entity.DisplayName = string.IsNullOrEmpty(wx_dept.name) ? entity.DisplayName : wx_dept.name;
                                _organizationUnitRepository.Update(entity);
                            }
                            break;
                        default:
                            break;
                    }

                }

            }
        }


        /// <summary>
        /// 同步单个组织         
        /// </summary>
        /// <param name="wx_dept"></param>
        /// <returns>更新的本地Id</returns>
        public void MatchSingleDepartment(SyncDepartment wx_dept, int? tenant_id)
        {
            if (wx_dept != null)
            {
                var entity = _organizationUnitRepository.FirstOrDefault(o => o.WXDeptId == wx_dept.id && o.TenantId == tenant_id);
                var result_id = entity?.Id ?? null;
                var parent_entity = wx_dept.parentid == 0 ? null : (_organizationUnitRepository.FirstOrDefault(o => o.WXDeptId == wx_dept.parentid && o.TenantId == tenant_id));
                var parent_id = parent_entity?.Id ?? null;
                var parent_code = parent_entity?.Code ?? "";

                switch (wx_dept.changetype)
                {
                    case "delete_party":
                        if (result_id.HasValue)
                            _organizationUnitRepository.Delete(result_id.Value);
                        break;
                    case "create_party":
                        if (entity == null)
                        {
                            entity = new AbpOrganizationUnitExtend
                            {
                                TenantId = tenant_id,//AbpSession.TenantId,
                                WXDeptId = wx_dept.id,
                                DisplayName = wx_dept.name,
                                WXParentDeptId = wx_dept.parentid,
                                ParentId = parent_id,
                                Code = ""
                            };
                            result_id = _organizationUnitRepository.InsertAndGetId(entity);
                            //CurrentUnitOfWork.SaveChanges();
                            entity.Code = string.IsNullOrEmpty(parent_code) ? result_id.ToString() : parent_code + ":" + result_id.Value;
                            entity.Id = result_id.Value;
                            _organizationUnitRepository.Update(entity);
                        }
                        break;
                    case "update_party":
                        if (result_id.HasValue)
                        {
                            /**
                             * 2019-04-28 miansheng.luo 企业微信回调有更新忽略。修改时请注意修改下列字段判断方式
                             */
                            if (wx_dept.parentid.HasValue)
                            {
                                entity.ParentId = parent_id;
                                entity.Code = string.IsNullOrEmpty(parent_code) ? result_id.ToString() : parent_code + ":" + result_id.Value;
                                entity.WXParentDeptId = wx_dept.parentid;
                            }
                            entity.TenantId = tenant_id;
                            entity.WXDeptId = wx_dept.id;
                            entity.DisplayName = string.IsNullOrEmpty(wx_dept.name) ? entity.DisplayName : wx_dept.name;
                            _organizationUnitRepository.Update(entity);
                        }
                        break;
                    default:
                        break;
                }

            }
        }


        /// <summary>
        /// 批量同步
        /// </summary>
        /// <param name="wx_departments"></param>
        /// <returns></returns>
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


        #region  Synchronize User

        /// <summary>
        /// 同步单个用户(无租户验证版本)
        /// </summary>
        /// <param name="wx_user"></param>
        /// <param name="tenant_id"></param>
        /// <returns>更新的本地Id</returns>
        public void MatchSingleUserWithoutTenant(SyncUser wx_user, int? tenant_id)
        {
            if (wx_user != null)
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {

                    var ul = _userLoginRepository.FirstOrDefault(x => x.ProviderKey == wx_user.userid && x.LoginProvider == "Wechat" && x.TenantId == tenant_id);
                    //delete_user
                    switch (wx_user.changetype)
                    {
                        case "delete_user":
                            if (ul != null)
                            {
                                _userOrganizationUnitRepository.Delete(x => x.UserId == ul.UserId);
                                _userRoleRepository.Delete(x => x.UserId == ul.UserId);
                                _userRepository.Delete(ul.UserId);
                                _userLoginRepository.Delete(ul);
                            }
                            break;
                        case "create_user":
                            if (ul == null)
                            {
                                var user_name = string.IsNullOrEmpty(wx_user.email) ? "" : wx_user.email.Split('@')[0];
                                //先检查用户是否有重复
                                if (_userRepository.GetAll().Any(x => x.UserName == wx_user.email || x.UserName == user_name || x.EmailAddress == wx_user.email))
                                {
                                    _logger.Info(Abp.Timing.Clock.Now.ToString("yyyy-MM-dd HH:mm:ss") + wx_user.email + " : 用户已存在,无法通过企业微信回调接口Insert");
                                    return;
                                }
                                var user = new User
                                {
                                    TenantId = tenant_id,
                                    Name = wx_user.name,
                                    NormalizedUserName = wx_user.name,
                                    PhoneNumber = wx_user.mobile,
                                    EmailAddress = wx_user.email,
                                    NormalizedEmailAddress = wx_user.email,
                                    AccessFailedCount = 0,
                                    IsDeleted = false,
                                    IsActive = true,
                                    UserName = user_name,
                                    IsEmailConfirmed = true,
                                    IsTwoFactorEnabled = true,
                                    IsPhoneNumberConfirmed = !string.IsNullOrEmpty(wx_user.mobile),

                                    IsLockoutEnabled = true,
                                    Avatar = wx_user.avatar,
                                    Position = wx_user.position,
                                    Sex = wx_user.gender == "1" ? true : false,
                                    Surname = wx_user.alias ?? "",
                                };

                                user.Password = _passwordHasher.HashPassword(user, "000000");
                                var new_id = _userRepository.InsertAndGetId(user);

                                _userLoginRepository.Insert(new UserLogin { LoginProvider = "Wechat", ProviderKey = wx_user.userid, TenantId = tenant_id, UserId = new_id });

                                CurrentUnitOfWork.SaveChanges();

                                if (!string.IsNullOrEmpty(wx_user.department))
                                {
                                    var department_list = wx_user.department.Split(',');
                                    var local_dept = _organizationUnitRepository.GetAll().Where(x => department_list.Contains(x.WXDeptId.ToString()));
                                    if (local_dept.Any())
                                    {
                                        foreach (var item in local_dept)
                                        {
                                            _userOrganizationUnitRepository.Insert(new UserOrganizationUnit { TenantId = tenant_id, UserId = new_id, OrganizationUnitId = item.Id });
                                        }
                                        CurrentUnitOfWork.SaveChanges();
                                    }
                                }
                            }
                            break;
                        case "update_user":
                            if (ul != null)
                            {
                                var user_id = ul.UserId;
                                var entity = _userRepository.Get(user_id);
                                entity.TenantId = tenant_id;
                                entity.Name = wx_user.name ?? entity.Name;
                                entity.EmailAddress = wx_user.email ?? entity.EmailAddress;
                                entity.PhoneNumber = wx_user.mobile ?? entity.PhoneNumber;
                                entity.Avatar = wx_user.avatar ?? entity.Avatar;
                                entity.Position = wx_user.position ?? entity.Position;
                                entity.Surname = wx_user.alias ?? entity.Surname;

                                entity.Sex = wx_user.gender == null ? entity.Sex : (wx_user.gender == "1" ? true : false);
                                entity.IsActive = wx_user.status.HasValue ? wx_user.status == 1 : entity.IsActive;

                                CurrentUnitOfWork.SaveChanges();

                                if (!string.IsNullOrEmpty(wx_user.department))
                                {

                                    //先删除所有关联信息
                                    //_userOrganizationUnitRepository.Delete(x => x.UserId == ul.UserId);
                                    //CurrentUnitOfWork.SaveChanges();

                                    //var department_list = wx_user.department.Split(',');

                                    var wx_dept = wx_user.department.Split(',').ToList();
                                    var local_dept = (from o in _organizationUnitRepository.GetAll()
                                                      where o.TenantId == tenant_id && wx_dept.Contains(o.WXDeptId.ToString())
                                                      select o.Id).ToList();
                                    //_organizationUnitRepository.GetAll().Where(x => x.TenantId == tenant_id && wx_dept.Contains(x.WXDeptId.ToString())).ToList();


                                    //先删除 微信没有的
                                    _userOrganizationUnitRepository.Delete(x => x.UserId == user_id && !local_dept.Contains(x.OrganizationUnitId));//!local_dept.Any(d => x.OrganizationUnitId == d.Id));
                                    CurrentUnitOfWork.SaveChanges();

                                    var local_user_dept = _userOrganizationUnitRepository.GetAll().Where(x => x.TenantId == tenant_id && x.UserId == user_id).ToList();

                                    //再添加 本地没有的
                                    foreach (var item in local_dept)
                                    {
                                        if (!local_user_dept.Any(x => x.OrganizationUnitId == item))
                                        {
                                            _userOrganizationUnitRepository.Insert(new UserOrganizationUnit { TenantId = tenant_id, UserId = user_id, OrganizationUnitId = item });
                                        }
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        #endregion


        #region Synchronize Tag

        /// <summary>
        /// 同步标签 (无租户验证版本)         
        /// </summary>
        /// <param name="wx_dept"></param>
        /// <returns>更新的本地Id</returns>
        public void MatchQYTagWithoutTenant(SyncTag wx_tag, int? tenant_id)
        {
            if (wx_tag != null)
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    if (wx_tag.ChangeType == "update_tag")
                    {
                        _logger.Info("update : " + wx_tag.AddUserItems + " del : " + wx_tag.DelUserItems);
                        var local_role = _roleRepository.FirstOrDefault(r => r.WxTagId == wx_tag.TagId && r.TenantId == tenant_id);
                        int role_id = 0;
                        if (local_role == null)
                        {
                            var tag_name = _weChatManager.GetTagNameById(wx_tag.TagId.Value, tenant_id.Value);
                            local_role = new Role
                            {
                                WxTagId = wx_tag.TagId,
                                IsDefault = false,
                                NormalizedName = "WX_" + tag_name,
                                DisplayName = "WX_" + tag_name,
                                Name = "WX_" + tag_name,
                                TenantId = tenant_id
                            };
                            role_id = _roleRepository.InsertAndGetId(local_role);
                            CurrentUnitOfWork.SaveChanges();
                        }
                        else
                        {
                            role_id = local_role.Id;
                        }

                        if (!string.IsNullOrEmpty(wx_tag.DelUserItems))
                        {
                            var del_users = wx_tag.DelUserItems.Split(',');
                            var del_list = _userLoginRepository.GetAll().Where(x => del_users.Contains(x.ProviderKey) && x.LoginProvider == "Wechat" && x.TenantId == tenant_id).ToList();
                            if (del_list?.Count > 0)
                            {
                                _userRoleRepository.Delete(x => x.RoleId == role_id && del_list.Any(d => d.UserId == x.UserId));
                                CurrentUnitOfWork.SaveChanges();
                            }
                        }

                        if (!string.IsNullOrEmpty(wx_tag.AddUserItems))
                        {
                            var add_users = wx_tag.AddUserItems.Split(',');
                            var add_list = _userLoginRepository.GetAll().Where(x => add_users.Contains(x.ProviderKey) && x.LoginProvider == "Wechat" && x.TenantId == tenant_id).ToList();
                            if (add_list?.Count > 0)
                            {
                                foreach (var item in add_list)
                                {
                                    if (!_userRoleRepository.GetAll().Any(x => x.RoleId == role_id && x.TenantId == tenant_id && x.UserId == item.UserId))
                                    {
                                        _userRoleRepository.Insert(new UserRole
                                        {
                                            RoleId = role_id,
                                            TenantId = tenant_id,
                                            UserId = item.UserId
                                        });
                                    }
                                }
                                CurrentUnitOfWork.SaveChanges();
                            }
                        }
                    }

                }
            }
        }


        #endregion

    }
}
