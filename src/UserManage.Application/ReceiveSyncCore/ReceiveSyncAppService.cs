using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Organizations;
using Abp.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManage.AbpExternalCore.DomainService;
using UserManage.AbpOrganizationUnitCore;
using UserManage.Authorization.Roles;
using UserManage.Authorization.Users;
using UserManage.BaseEntityCore;
using UserManage.ReceiveSyncCore.Dtos;
using UserManage.Users.Dto;

namespace UserManage.ReceiveSyncCore
{   
    /// <summary>
    /// 接收同步
    /// </summary>
    public class ReceiveSyncAppService : UserManageAppServiceBase, IReceiveSyncAppService
    {
        private readonly IAbpWeChatManager _weChatManager;

        //用户
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<UserLogin, long> _userLoginRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRepository<BaseUserEmp, long> _baseUserRepository;

        //组织
        private readonly IRepository<AbpOrganizationUnitExtend, long> _organizationUnitRepository;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;
        private readonly OrganizationUnitManager _organizationUnitManager;
        private readonly IRepository<BaseUserOrg, int> _baseOrgRepository;
        private readonly IRepository<BaseUserEmpOrg, int> _baseEmpOrgRepository;

        ////角色
        private readonly IRepository<Role, int> _roleRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<BaseUserRole, int> _baseRoleRepository;

        public ReceiveSyncAppService(
            IAbpWeChatManager weChatManager
            , IRepository<User, long> userRepository
            , IRepository<UserLogin, long> userLoginRepository
            , IPasswordHasher<User> passwordHasher
            , IRepository<AbpOrganizationUnitExtend, long> organizationUnitRepository
            , IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository
            , OrganizationUnitManager organizationUnitManager
            , IRepository<Role, int> roleRepository
            , IRepository<UserRole, long> userRoleRepository
            , IRepository<BaseUserEmp, long> baseUserRepository
            , IRepository<BaseUserOrg, int> baseOrgRepository
            , IRepository<BaseUserEmpOrg, int> baseEmpOrgRepository
            , IRepository<BaseUserRole, int> baseRoleRepository)
        {
            _weChatManager = weChatManager;
            _userRepository = userRepository;
            _userLoginRepository = userLoginRepository;
            _passwordHasher = passwordHasher;
            _organizationUnitRepository = organizationUnitRepository;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _organizationUnitManager = organizationUnitManager;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _baseUserRepository = baseUserRepository;
            _baseOrgRepository = baseOrgRepository;
            _baseEmpOrgRepository = baseEmpOrgRepository;
            _baseRoleRepository = baseRoleRepository;
        }

        #region Private

        private async Task<User> GetUserByWorkWeinxinId(string wxId)
        {
            var current_tenantid = 2;
            using (CurrentUnitOfWork.SetTenantId(current_tenantid))
            {
                var login_user = await _userLoginRepository.GetAll().FirstOrDefaultAsync(x =>
                    x.TenantId == current_tenantid && x.LoginProvider == "Wechat" && x.ProviderKey == wxId);
                if (login_user != null)
                {
                    return await _userRepository.FirstOrDefaultAsync(login_user.UserId);
                }
                return null;
            }
        }



        /// <summary>
        /// 外部导入User
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<long> AuthCreate(CreateUserDto input)
        {
            //CheckCreatePermission();

            var user = ObjectMapper.Map<User>(input);

            user.TenantId = AbpSession.TenantId;
            user.Password = _passwordHasher.HashPassword(user, input.Password);
            user.IsEmailConfirmed = true;


            await UserManager.CreateAsync(user);


            CurrentUnitOfWork.SaveChanges();

            return user.Id;

        }

        #endregion

        #region 用户

        

        /// <summary>
        /// 接收单一用户(新增同步F2BPM)
        /// </summary>
        /// <returns></returns>
        [DontWrapResult]
        public async Task<ReceiveSyncResultDto> SyncSingleUser(ReceiveSyncUserDto input)
        {
            var current_tenantid = 2;
            var action = input.ActionName.ToLower();

            using (CurrentUnitOfWork.SetTenantId(current_tenantid))
            {
                try
                {
                    var user = await this.GetUserByWorkWeinxinId(input.WXId);
                    if (action == "create_user")
                    {
                        if(user != null) return new ReceiveSyncResultDto(10, "用户已存在");
                        user = ObjectMapper.Map<User>(input.Data);

                        user.SetNormalizedNames();
                        user.TenantId = current_tenantid;
                        user.Password = _passwordHasher.HashPassword(user, "000000");
                        user.IsEmailConfirmed = true;


                        await UserManager.CreateAsync(user);

                        CurrentUnitOfWork.SaveChanges();

                        var user_id = user.Id;

                        await _userLoginRepository.InsertAsync(new UserLogin
                        {
                            UserId = user_id,
                            LoginProvider = "Wechat",
                            ProviderKey = input.WXId,
                            TenantId = current_tenantid
                        });

                        await _baseUserRepository.InsertAsync(new BaseUserEmp
                        {
                            AbpUserId = user_id,
                            EmpOrderNo = user_id.ToString(),
                            EmpStationId = "",
                            EmpStatus = "1",
                            EmpUserGuid = Guid.NewGuid().ToString("N"),
                            IsLeader = "",
                            EmpUserId = input.WXId
                        });
                    }
                    else if (action == "modify_user")
                    {
                        if (user == null) return new ReceiveSyncResultDto(11, "用户不存在");
                        user.EmailAddress = input.Data.EmailAddress;
                        user.Surname = input.Data.Surname;
                        user.Avatar = input.Data.Avatar;
                        user.Position = input.Data.Position;
                        user.Name = input.Data.Name;
                        user.Sex = input.Data.Sex;
                        user.SelectDistrict = input.Data.SelectDistrict;
                        user.IsActive = input.Data.IsActive;
                        user.PhoneNumber = input.Data.PhoneNumber;
                        await UserManager.UpdateAsync(user);
                    }
                    else if (action == "delete_user")
                    {
                        if (user == null) return new ReceiveSyncResultDto(11, "用户不存在");
                        await _userRepository.DeleteAsync(user);
                        
                    }
                    else
                    {
                        return new ReceiveSyncResultDto(2, "无法识别Action");
                    }
                    return new ReceiveSyncResultDto(0, "success");
                }
                catch (Exception ex)
                {
                    return new ReceiveSyncResultDto(99, ex.Message);
                }
            }
        }

        #endregion


        #region 角色

        /// <summary>
        /// 同步单一角色
        /// </summary>
        /// <returns></returns>
        [DontWrapResult]
        public async Task<ReceiveSyncResultDto> SyncSingleRole(ReceiveSyncRoleDto input)
        {
            var current_tenantid = 2;
            var action = input.ActionName.ToLower();
            using (CurrentUnitOfWork.SetTenantId(current_tenantid))
            {
                var entity = await _roleRepository.FirstOrDefaultAsync(x => x.WxTagId == input.WxTagId);
                
                if (action == "create_role")
                {
                    if (entity != null) return new ReceiveSyncResultDto(20, "角色已存在");
                    await RoleManager.CreateAsync(new Role
                    {
                        WxTagId = input.WxTagId,
                        IsDefault = false,
                        NormalizedName = input.RoleName,
                        DisplayName = input.RoleName,
                        Name = input.RoleName,
                        TenantId = current_tenantid
                    });
                }
                else if (action == "modify_role")
                {
                    if (entity == null) return new ReceiveSyncResultDto(21, "角色不存在");
                    entity.NormalizedName = input.RoleName;
                    entity.DisplayName = input.RoleName;
                    entity.Name = input.RoleName;
                    //entity.WxTagId = item.WechatTagId;
                    await RoleManager.UpdateAsync(entity);
                }
                else if (action == "delete_role")
                {
                    if (entity == null) return new ReceiveSyncResultDto(21, "角色不存在");
                    var any_user = await _userRoleRepository.GetAll().AnyAsync(x => x.RoleId == entity.Id);
                    if (any_user) return new ReceiveSyncResultDto(22, "角色内仍有用户");
                    await RoleManager.DeleteAsync(entity);
                }
                else
                {
                    return new ReceiveSyncResultDto(2, "无法识别Action");
                }
                return new ReceiveSyncResultDto(0, "");
            }
        }


        #endregion

        #region 部门

        /// <summary>
        /// 同步单一部门(新增修改同步F2BPM)
        /// </summary>
        /// <returns></returns>
        [DontWrapResult]
        public async Task<ReceiveSyncResultDto> SyncSingleOrganizationUnits(ReceiveSyncOrganizationDto input)
        {
            var current_tenantid = 2;
            var action = input.ActionName.ToLower();

            using (CurrentUnitOfWork.SetTenantId(current_tenantid))
            {
                var parent = await _organizationUnitRepository.FirstOrDefaultAsync(x => x.WXDeptId == input.WXParentDeptId); 
                if (action != "delete_dept")
                {
                    if (parent == null) return new ReceiveSyncResultDto(33, "父级部门不存在"); 
                }

                var entity = await _organizationUnitRepository.FirstOrDefaultAsync(x => x.WXDeptId == input.WXDeptId);
                var base_org = _baseOrgRepository.FirstOrDefault(o => o.WxId == input.WXDeptId.ToString());
                var parent_org = _baseOrgRepository.FirstOrDefault(x => x.WxId == input.WXParentDeptId.ToString());
                if (action == "create_dept")
                {
                    if (entity != null) return new ReceiveSyncResultDto(30, "部门已存在");
                    entity = new AbpOrganizationUnitExtend
                    {
                        TenantId = current_tenantid,//AbpSession.TenantId,
                        WXDeptId = input.WXDeptId,
                        DisplayName = input.DisplayName,
                        WXParentDeptId = input.WXParentDeptId,
                        ParentId = parent.Id,
                        Code = ""
                    };
                    var result_id = _organizationUnitRepository.InsertAndGetId(entity);
                    //CurrentUnitOfWork.SaveChanges();
                    entity.Code = string.IsNullOrEmpty(parent.Code) ? result_id.ToString() : parent.Code + ":" + result_id;
                    entity.Id = result_id;
                    _organizationUnitRepository.Update(entity);

                    if (parent.Id == 1)
                    {
                        _baseOrgRepository.Insert(new BaseUserOrg
                        {
                            Name = input.DisplayName,
                            OrgGuid = Guid.NewGuid().ToString(),
                            OrgOrderNo = input.WXDeptId,
                            OrgParentGuid = "0",
                            ParentId = 0,
                            WxId = input.WXDeptId.ToString()
                        });
                    }
                    else
                    {
                        
                        if (parent_org != null)
                        {
                            _baseOrgRepository.Insert(new BaseUserOrg
                            {
                                Name = input.DisplayName,
                                OrgGuid = Guid.NewGuid().ToString(),
                                OrgOrderNo = input.WXDeptId,
                                OrgParentGuid = parent_org.OrgGuid,
                                ParentId = parent_org.Id,
                                WxId = input.WXDeptId.ToString()
                            });
                        }
                    }
                }
                else if (action == "modify_dept")
                {
                    if (entity == null) return new ReceiveSyncResultDto(31, "部门不存在");
                    entity.TenantId = current_tenantid;
                    entity.DisplayName = input.DisplayName;
                    entity.WXParentDeptId = parent.WXDeptId;
                    entity.ParentId = parent.Id;
                    entity.Code = string.IsNullOrEmpty(parent.Code) ? entity.Id.ToString() : parent.Code + ":" + entity.Id;
                    await _organizationUnitRepository.UpdateAsync(entity);

                    base_org.Name = input.DisplayName;
                    base_org.OrgParentGuid = parent_org != null ? parent_org.OrgGuid : "0";
                    base_org.ParentId = parent_org != null ? parent_org.Id : 0;
                    _baseOrgRepository.Update(base_org);
                }
                else if (action == "delete_dept")
                {
                    if (entity == null) return new ReceiveSyncResultDto(31, "部门不存在");
                    var any_user = await _userOrganizationUnitRepository.GetAll().AnyAsync(x => x.OrganizationUnitId == entity.Id);
                    if (any_user) return new ReceiveSyncResultDto(32, "部门内仍有用户");
                    await _organizationUnitRepository.DeleteAsync(entity);
                    await _baseOrgRepository.DeleteAsync(base_org);
                    return new ReceiveSyncResultDto(0, "");
                }
                else
                {
                    return new ReceiveSyncResultDto(2, "无法识别Action");
                }

                return new ReceiveSyncResultDto(0, "");
            }
        }

        
        #endregion

    }
}
