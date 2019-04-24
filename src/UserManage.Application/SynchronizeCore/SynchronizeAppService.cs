using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManage.AbpExternalCore;
using UserManage.AbpExternalCore.DomainService;
using UserManage.AbpOrganizationUnitCore;
using UserManage.Authorization.Users;
using UserManage.SynchronizeCore.Dtos;

namespace UserManage.SynchronizeCore
{
    /// <summary>
    /// 同步应用层服务的接口实现方法  
    ///</summary>    
    [AbpAuthorize]
    public class SynchronizeAppService : UserManageAppServiceBase, ISynchronizeAppService
    {
        private readonly IAbpWeChatManager _weChatManager;

        //组织
        private readonly IRepository<AbpOrganizationUnitExtend, long> _organizationUnitRepository;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;

        //用户
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<UserLogin, long> _userLoginRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        ////标签
        //private readonly IRepository<AbpTag, int> _tagRepository;
        //private readonly IRepository<AbpUserTag, int> _userTagRepository;

        ////角色
        //private readonly IRepository<Role, int> _roleRepository;
        //private readonly IRepository<UserRole, long> _userRoleRepository;

        public SynchronizeAppService(
            IAbpWeChatManager weChatManager,
            IRepository<AbpOrganizationUnitExtend, long> organizationUnitRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IRepository<User, long> userRepository,
            IRepository<UserLogin, long> userLoginRepository,
            IPasswordHasher<User> passwordHasher
        )
        {
            _weChatManager = weChatManager;
            _organizationUnitRepository = organizationUnitRepository;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _userRepository = userRepository;
            _userLoginRepository = userLoginRepository;
            _passwordHasher = passwordHasher;
        }


        /// <summary>
        /// 同步所有用户与组织关联信息
        /// </summary>
        /// <returns></returns>
        public async Task<ManyMatchResultDto> MatchDeptAndUser()
        {
            ManyMatchResultDto results = new ManyMatchResultDto();

            results.results.Add("组织", await this.MatchDepartment());
            results.results.Add("用户", await this.MatchUser());
            results.results.Add("用户与组织关联", await this.MatchDepartmentUsers());

            return results;
        }


        #region  Synchronize Department

        /// <summary>
        /// 同步企业微信 部门 => 组织
        /// </summary>
        /// <returns></returns>
        public async Task<MatchResultDto> MatchDepartment()
        {
            MatchResultDto result = new MatchResultDto { CreateCount = 0, DeleteCount = 0, MatchCount = 0 };
            var current_tenantid = this.AbpSession.TenantId;
            //获取企业微信部门
            var wx_departments = await _weChatManager.GetAllDepartments();

            if (wx_departments != null)
            {
                // 尝试获取企业微信部门与本地组织full join
                var wx_abp_depts = this.GetWxAbpDepartments(wx_departments);
                if (wx_abp_depts != null)
                {
                    //循环Full Join 列表
                    foreach (var item in wx_abp_depts)
                    {
                        //判断是否拥有本地Id
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
                                TenantId = current_tenantid,
                                WXDeptId = item.wx_id,
                                DisplayName = item.wx_name,
                                WXParentDeptId = item.wx_parentid,
                                Code = ""
                            });
                            result.CreateCount++;
                        }

                    }
                    //提交增删改记录
                    CurrentUnitOfWork.SaveChanges();

                    //重新更新上下级关系
                    var local_list = await _organizationUnitRepository.GetAll().ToListAsync();
                    var begin_codes = local_list.Where(x => x.WXParentDeptId == 0);
                    foreach (var item in begin_codes)
                    {
                        await GetLocalDepartment(local_list, item.WXDeptId, item.Id, item.Id.ToString());
                        item.Code = item.Id.ToString();
                        await _organizationUnitRepository.UpdateAsync(item);
                    }
                    result.MatchCount = local_list.Count;
                    CurrentUnitOfWork.SaveChanges();
                }
            }

            return result;
        }

        /// <summary>
        /// 同步用户部门关联
        /// </summary>
        /// <returns></returns>
        public async Task<MatchResultDto> MatchDepartmentUsers()
        {
            MatchResultDto result = new MatchResultDto { CreateCount = 0, DeleteCount = 0, MatchCount = 0 };
            var relations = await _weChatManager.GetAllUsersDeptRelation();
            if (relations != null)
            {
                var current_tenantid = this.AbpSession.TenantId;
                var query_list = (from ul in _userLoginRepository.GetAll()
                                  join r in relations.AsQueryable() on new { key = ul.ProviderKey, type = ul.LoginProvider } equals new { key = r.Item1, type = "Wechat" }
                                  join d in _organizationUnitRepository.GetAll() on r.Item2 equals d.WXDeptId
                                  select new UserOrganizationUnit
                                  {
                                      IsDeleted = false,
                                      TenantId = current_tenantid,
                                      OrganizationUnitId = d.Id,
                                      UserId = ul.UserId
                                  }).ToList();
                if (query_list?.Count > 0)
                {
                    await _userOrganizationUnitRepository.DeleteAsync(x => x.TenantId == current_tenantid);

                    foreach (var item in query_list)
                    {
                        await _userOrganizationUnitRepository.InsertAsync(item);
                        result.CreateCount++;
                    }
                    CurrentUnitOfWork.SaveChanges();
                }
            }
            return result;
        }

        /// <summary>
        /// 重构上下级关系(递归)
        /// </summary>
        /// <returns></returns>
        private async Task GetLocalDepartment(List<AbpOrganizationUnitExtend> units, int? begin_wx_id, long parent_id, string code)
        {
            var begin_codes = units.Where(x => x.WXParentDeptId == begin_wx_id);
            if (begin_codes.Any())
            {
                foreach (var item in begin_codes)
                {
                    var new_code = code + ":" + item.Id.ToString();
                    item.Code = new_code;
                    item.ParentId = parent_id;
                    await _organizationUnitRepository.UpdateAsync(item);
                    await GetLocalDepartment(units, item.WXDeptId, item.Id, new_code);
                }
            }
        }

        /// <summary>
        /// 再次尝试获取企业微信部门与本地组织full join
        /// </summary>
        /// <param name="wx_dept"></param>
        /// <returns></returns>
        private List<AbpWxDepartmentsDto> GetWxAbpDepartments(ICollection<AbpWeChatDepartment> wx_dept)
        {

            //var o_query = _organizationUnitRepository.GetAll().ToList();
            var left_query = from o in _organizationUnitRepository.GetAll()
                             join dm in wx_dept.AsQueryable() on o.WXDeptId equals dm.id into dori
                             from d in dori.DefaultIfEmpty()
                             where o.TenantId == AbpSession.TenantId
                             select new AbpWxDepartmentsDto
                             {
                                 abp_id = o.Id,
                                 wx_id = (d == null ? 0 : d.id),
                                 wx_name = (d == null ? "" : d.name),
                                 wx_parentid = (d == null ? 0 : d.parentid),
                             };

            var right_query = from d in wx_dept.AsQueryable()
                              where !_organizationUnitRepository.GetAll().Any(o => o.WXDeptId == d.id && o.TenantId == AbpSession.TenantId)
                              select new AbpWxDepartmentsDto
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

        #region Synchronize User

        /// <summary>
        /// 再次尝试获取企业微信用户与本地用户full join
        /// </summary>
        /// <param name="wx_dept"></param>
        /// <returns></returns>
        private List<AbpWxUserDto> GetWxAbpUsers(ICollection<AbpWeChatUser> wx_users)
        {
            var left_query = from u in _userRepository.GetAll()
                             join ul in _userLoginRepository.GetAll() on u.Id equals ul.UserId
                             join wum in wx_users.AsQueryable() on new { key = ul.ProviderKey, type = ul.LoginProvider } equals new { key = wum.userid, type = "Wechat" } into wuo
                             from wu in wuo.DefaultIfEmpty()
                             where u.TenantId == AbpSession.TenantId
                             select new AbpWxUserDto
                             {
                                 AbpUserId = u.Id,
                                 AbpUserName = u.UserName,
                                 wx_alias = (wu == null ? "" : wu.alias),
                                 wx_avatar = (wu == null ? "" : wu.avatar),
                                 wx_email = (wu == null ? "" : wu.email),
                                 wx_gender = (wu == null ? "" : wu.gender),
                                 wx_mobile = (wu == null ? "" : wu.mobile),
                                 wx_name = (wu == null ? "" : wu.name),
                                 wx_position = (wu == null ? "" : wu.position),
                                 wx_qr_code = (wu == null ? "" : wu.qr_code),
                                 wx_userid = (wu == null ? "" : wu.userid)
                             };

            var right_query = from wu in wx_users.AsQueryable()
                              where !_userLoginRepository.GetAll().Any(ul => ul.ProviderKey == wu.userid && ul.LoginProvider == "Wechat" && ul.TenantId == AbpSession.TenantId)
                              select new AbpWxUserDto
                              {
                                  AbpUserId = 0,
                                  AbpUserName = wu.email.Substring(0, wu.email.IndexOf('@')),
                                  wx_alias = wu.alias,
                                  wx_avatar = wu.avatar,
                                  wx_email = wu.email,
                                  wx_gender = wu.gender,
                                  wx_mobile = wu.mobile,
                                  wx_name = wu.name,
                                  wx_position = wu.position,
                                  wx_qr_code = wu.qr_code,
                                  wx_userid = wu.userid
                              };

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

        /// <summary>
        /// 根据企业微信导入用户
        /// </summary>
        /// <returns></returns>
        public async Task<MatchResultDto> MatchUser()
        {
            //初始化结果[添加用户数,新建关联数,同步后离职人员数量]
            MatchResultDto result = new MatchResultDto { CreateCount = 0, MatchCount = 0, DeleteCount = 0 };
            try
            {
                var top_dept = await _organizationUnitRepository.FirstOrDefaultAsync(x => x.WXParentDeptId == 0);
                if (top_dept != null)
                {
                    //企业微信用户list
                    var elist = await _weChatManager.GetAllUsersByDepartment(1);
                    if (elist != null)
                    {
                        var full_list = this.GetWxAbpUsers(elist);
                        if (full_list?.Count > 0)
                        {
                            foreach (var item in full_list)
                            {
                                if (item.AbpUserName.ToLower().Equals("admin"))
                                    continue;
                                if (item.AbpUserId != 0)
                                {
                                    var entity = await UserManager.GetUserByIdAsync(item.AbpUserId.Value);
                                    if (string.IsNullOrEmpty(item.wx_userid))
                                    {
                                        await UserManager.DeleteAsync(entity);
                                        result.DeleteCount++;
                                    }
                                    else
                                    {
                                        entity.EmailAddress = item.wx_email;
                                        entity.Surname = item.wx_alias;
                                        entity.Name = item.wx_name;
                                    }
                                }
                                else
                                {
                                    var user_id = await this.AuthCreate(new CreateAuthUserDto()
                                    {
                                        EmailAddress = item.wx_email,
                                        Name = item.wx_name,
                                        PhoneNumber = item.wx_mobile,
                                        IsActive = true,
                                        Position = item.wx_position,
                                        Sex = item.wx_gender == "1" ? true : false,
                                        Surname = item.wx_alias,
                                        UserName = item.AbpUserName,
                                        Password = "000000"
                                    });
                                    result.CreateCount++;
                                    await _userLoginRepository.InsertAsync(new UserLogin
                                    {
                                        UserId = user_id,
                                        LoginProvider = "Wechat",
                                        ProviderKey = item.wx_userid,
                                        TenantId = AbpSession.TenantId
                                    });
                                    result.MatchCount++;
                                }
                            }
                            CurrentUnitOfWork.SaveChanges();
                        }
                    }
                }
                #region 旧代码注释

                //if (elist.Any())
                //{
                //    //abp用户包含微信用户的list
                //    var ulist = await Repository.GetAll().ToListAsync();
                //    var userLoginList = await _userLoginRepository.GetAll().Where(ul => ul.LoginProvider == "Wechat").ToListAsync();

                //    foreach (var item in elist)
                //    {
                //        if (string.IsNullOrEmpty(item.email)) { continue; }

                //        var uName = item.email.Split('@')[0];

                //        var euser = ulist.FirstOrDefault(x => x.Name == item.name && x.EmailAddress == item.email);
                //        //假设userid = admin ,则将用户名强制为email
                //        if (item.userid.ToLower() == "admin" && euser == null)
                //        {
                //            euser = ulist.FirstOrDefault(x => x.UserName == uName);
                //            euser.Name = item.name;
                //            euser.EmailAddress = item.email;
                //            //CheckErrors(await _userManager.UpdateAsync(euser));
                //        }

                //        if (euser == null)
                //        {
                //            //userid = await this.AuthCreate(new CreateAuthUserDto()
                //            //{
                //            //    EmailAddress = item.email,
                //            //    Name = item.name,
                //            //    PhoneNumber = item.mobile,
                //            //    IsActive = true,
                //            //    Position = item.position,
                //            //    Sex = item.gender == "1" ? true : false,
                //            //    Surname = item.alias,
                //            //    UserName = uName,
                //            //    Password = "000000"
                //            //});
                //            i++;
                //        }
                //        else
                //        {
                //            userid = euser.Id;
                //        }


                //        if (!userLoginList.Any(x => x.UserId == userid && x.ProviderKey == item.userid && x.TenantId == AbpSession.TenantId))
                //        {
                //            //await _userLoginRepository.InsertAsync(new UserLogin
                //            //{
                //            //    UserId = userid,
                //            //    LoginProvider = "Wechat",
                //            //    ProviderKey = item.userid,
                //            //    TenantId = AbpSession.TenantId
                //            //});
                //            //CurrentUnitOfWork.SaveChanges();
                //            j++;
                //        }
                //        userid = 0;

                //    }
                //    var noWeChatList = await Repository.GetAll().Where(u => !_userLoginRepository.GetAll().Any(l => l.UserId == u.Id)).ToListAsync();
                //    if (noWeChatList.Any())
                //    {
                //        foreach (var item in noWeChatList)
                //        {
                //            //await Repository.DeleteAsync(item.Id);
                //            //CurrentUnitOfWork.SaveChanges();
                //            k++;
                //        }
                //    }

                //}

                #endregion
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }

            //return new ListResultDto<UserListDto>(resultDto);
        }



        /// <summary>
        /// 外部导入User
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<long> AuthCreate(CreateAuthUserDto input)
        {
            //CheckCreatePermission();

            var user = ObjectMapper.Map<User>(input);

            user.TenantId = AbpSession.TenantId;
            user.Password = _passwordHasher.HashPassword(user, input.Password);
            user.IsEmailConfirmed = true;

            await UserManager.CreateAsync(user);


            //CurrentUnitOfWork.SaveChanges();

            return user.Id;

        }

        #endregion


    }
}
