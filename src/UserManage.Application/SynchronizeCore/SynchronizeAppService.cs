using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManage.AbpExternalCore;
using UserManage.AbpExternalCore.DomainService;
using UserManage.AbpOrganizationUnitCore;
using UserManage.Authorization.Roles;
using UserManage.Authorization.Users;
using UserManage.BaseEntityCore;
using UserManage.BaseEntityCore.DomainService;
using UserManage.QYEmail.DomainService;
using UserManage.SynchronizeCore.Dtos;
using UserManage.Users.Dto;
using UserManage.Web;

namespace UserManage.SynchronizeCore
{
    /// <summary>
    /// 同步应用层服务的接口实现方法  
    ///</summary>    
    [AbpAuthorize]
    public class SynchronizeAppService : UserManageAppServiceBase, ISynchronizeAppService
    {
        private readonly IAbpWeChatManager _weChatManager;

        private readonly DomainService.ISynchronizeManager _testManager;


        //组织
        private readonly IRepository<AbpOrganizationUnitExtend, long> _organizationUnitRepository;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;
        private readonly IRepository<BaseUserEmpOrg, int> _baseEmpOrgRepository;

        //用户
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<UserLogin, long> _userLoginRepository;
        private readonly IPasswordHasher<User> _passwordHasher;


        ////角色
        private readonly IRepository<Role, int> _roleRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;

        //BaseUser
        private readonly IRepository<BaseUserEmp, long> _baseUserRepository;
        private readonly IBaseUserEmpManager _baseUserManager;


        private readonly QYEmailManager _qyEmailManager;
        private readonly ThirdPartyConfigCore.DomainService.ThirdPartyManager _tpManager;

        public SynchronizeAppService(
            IAbpWeChatManager weChatManager,
            DomainService.ISynchronizeManager testManager,
            IRepository<AbpOrganizationUnitExtend, long> organizationUnitRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IRepository<User, long> userRepository,
            IRepository<UserLogin, long> userLoginRepository,
            IRepository<BaseUserEmp, long> baseUserRepository,
            IRepository<Role, int> roleRepository,
            IRepository<UserRole, long> userRoleRepository,
            IPasswordHasher<User> passwordHasher,
            QYEmailManager qYEmailManager,
            ThirdPartyConfigCore.DomainService.ThirdPartyManager tpManager,
            IBaseUserEmpManager baseUserManager,
            IRepository<BaseUserEmpOrg, int> baseEmpOrgRepository
        )
        {
            _testManager = testManager;
            _weChatManager = weChatManager;
            _organizationUnitRepository = organizationUnitRepository;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _userRepository = userRepository;
            _baseUserRepository = baseUserRepository;
            _userLoginRepository = userLoginRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _passwordHasher = passwordHasher;
            _qyEmailManager = qYEmailManager;
            _tpManager = tpManager;
            _baseUserManager = baseUserManager;
            _baseEmpOrgRepository = baseEmpOrgRepository;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public void MatchTest()
        {
            //_testManager.MatchSingleDepartmentWithoutTenant(new SyncDepartment { changetype = "update_party", id = 166, name = "test", parentid = 10 }, 1);
            //var t =
            //_qyEmailManager.UpdateQYEmail(model, AbpSession.TenantId.Value, "update");
            var t = _qyEmailManager.GetEmailAllDepartment(2);//_tpManager.Test("E4B57428-BF03-493E-80B6-E38CECA47DD1");
        }


        /// <summary>
        /// 同步所有用户与组织关联信息
        /// </summary>
        /// <returns></returns>
        public async Task<ManyMatchResultDto> MatchDeptAndUser()
        {
            ManyMatchResultDto results = new ManyMatchResultDto();

            results.results.Add("组织", await this.MatchDepartment());
            results.results.Add("角色标签", await this.MatchTag());
            results.results.Add("用户", await this.MatchUser());
            results.results.Add("用户与组织关联", await this.MatchDepartmentUsers());
            results.results.Add("用户与角色标签关联", await this.MatchTagUser());
            return results;
        }

        #region GetWeChatUser

        /// <summary>
        /// 获取当前用户秘钥
        /// </summary>
        /// <returns></returns>
        public async Task<string> SearchSecretByCurrentUser()
        {
            if (!AbpSession.UserId.HasValue) throw new UserFriendlyException(99, "找不到当前用户信息");

            var query = from ul in _userLoginRepository.GetAll().AsNoTracking()
                            //join u in _userRepository.GetAll().AsNoTracking() on ul.UserId equals u.Id
                        where ul.LoginProvider == "Wechat" && ul.UserId == AbpSession.UserId.Value
                        select ul.ProviderKey;
            var wxid = await query.FirstOrDefaultAsync();
            if (wxid != null)
            {
                //先写死，日后扩大再更改配置
                var result = wxid.AESEncrypt("Wechat", "1000102");
                if (!result.isOk) return null;
                return result.text;
            }
            return null;
        }

        /// <summary>
        /// 根据用户ID
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<string> SearchWechatUserIdByAbpUserName(AbpUserNamesDto input)
        {
            if (input.AbpNames?.Count > 0)
            {
                var query = from ul in _userLoginRepository.GetAll().AsNoTracking()
                            join u in _userRepository.GetAll().AsNoTracking() on ul.UserId equals u.Id
                            where ul.LoginProvider == "Wechat" && input.AbpNames.Any(x => x == u.Name)
                            select ul.ProviderKey;
                return query.ToList();
            }
            return null;
        }

        #endregion

        #region change password

        /// <summary>
        /// 修改共享服务器密码
        /// </summary>
        /// <returns></returns>
        public async Task<string> ChangeACPassword(ChangePasswordInput input)
        {
            if (input.NewPassword.Length < 8)
            {
                throw new UserFriendlyException(99, "新密码必须在8位以上");
            }
            var abp_user = await _userRepository.GetAsync(input.AbpUserId);
            if (abp_user == null) throw new UserFriendlyException(99, "无效用户ID");

            //Dictionary<string, string> param = new Dictionary<string, string>();
            //param.Add("acct", abp_user.UserName);
            //param.Add("old", input.OldPassword);
            //param.Add("new", input.NewPassword);
            //param.Add("new2", input.RepeatPassword);
            var request_url = string.Format("https://192.168.168.118/iisadmpwd/achg.asp?acct={0}&old={1}&new={2}&new2={3}", abp_user.UserName, input.OldPassword, input.NewPassword, input.RepeatPassword);
            //var result = HttpMethods.RestGet(request_url);

            RestClient rest = new RestClient(request_url);
            var tcs = new TaskCompletionSource<string>();
            var request = new RestRequest(Method.GET);
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) =>
            {
                return true; //总是接受  
            });
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | (SecurityProtocolType)3072;

            //设置反序列化时预先处理乱码问题，如果调用的是Execute<T>方法，那么后面无需再次调用该方法
            request.OnBeforeDeserialization = res => RestSharpHelper.SetResponseEncoding(res, "gb2312");
            var response = rest.Execute(request);
            //直接读取Content还是需要调用该方法
            RestSharpHelper.SetResponseEncoding(response, "gb2312");

            var content = response.Content;
            if (content.Contains("缺少对象"))
            {
                return "修改失败:该共享服务器账号异常，或账号并未建立,请联系服务器管理员进行处理";//[关东成|张仕超]
            }
            else if (content.Contains("无效用户名或密码"))
            {
                return "修改失败:旧密码无效";
            }
            else if (content.Contains("密码成功更改"))
            {
                return "密码成功更改";
            }
            else if (content.Contains("密码太短"))
            {
                return "修改失败:密码太短,或不满足密码唯一性限制";
            }
            else if (content.Contains("密码不匹配"))
            {
                return "修改失败:新密码不一致";
            }
            else
            {
                return "修改失败:发生未知错误";
            }
        }

        #endregion

        #region Synchronize QYEmail
        /// <summary>
        /// 删除已匹配企业邮箱
        /// </summary>
        /// <returns></returns>
        public async Task MatchDeleteQyMail()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var users = await _userRepository.GetAll().Where(x => x.NormalizedUserName != "ADMIN" && !x.IsCreateEmail.Value).Select(x => x.EmailAddress).ToListAsync();
            }
        }

        /// <summary>
        /// 移除企业邮箱
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        public async Task RemoveQyMail(string email)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.SoftDelete))
            {
                using (CurrentUnitOfWork.SetTenantId(2))
                {
                    var entity = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.EmailAddress == email && x.IsDeleted);
                    if (entity != null)
                    {
                        var mail_entity = new QYEmail.QYMailUserInfocsForUpdate
                        {
                            userid = entity.EmailAddress,
                        };
                        _qyEmailManager.RemoveQYEmail(mail_entity, 2);
                    }
                }
            }
        }

        /// <summary>
        /// 业务系统 => 企业邮箱
        /// </summary>
        /// <returns></returns>
        public async Task MatchQyMail()
        {
            var users = await _userRepository.GetAll().Where(x => x.NormalizedUserName != "ADMIN" && !x.IsCreateEmail.Value).Select(x => x.EmailAddress ).ToListAsync();
            //var users = new List<string>();
            //users.Add("linxuehong@chinapsp.cn");

            var check_list = _qyEmailManager.CheckMailUser(users);


            foreach (var item in check_list)
            {
                if (item.type == 0)
                {
                    if (item.user.IndexOf("test") > -1) continue;
                    if (item.user.IndexOf("admin") > -1) continue;
                    var entity = await _userRepository.FirstOrDefaultAsync(x => x.EmailAddress == item.user);
                    var wx_id = (await _userLoginRepository.FirstOrDefaultAsync(x => x.UserId == entity.Id)).ProviderKey;
                    var mail_entity = new QYEmail.QYMailUserInfocsForUpdate
                    {
                        userid = entity.EmailAddress,
                        extid = wx_id,//entity.Id.ToString(),
                        department = new List<long>() { 6786316460997750890 },
                        position = entity.Position,
                        gender = entity.Sex ? "1" : "2",
                        mobile = entity.PhoneNumber,
                        name = entity.Name,
                    };
                    _qyEmailManager.UpdateQYEmail(mail_entity, this.AbpSession.TenantId.Value, "create");
                    entity.IsCreateEmail = true;
                    await _userRepository.UpdateAsync(entity);
                }
            }
        }



        /// <summary>
        /// 重置成默认密码
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        public async Task ResetQyMailPassword(string email)
        {
            using (CurrentUnitOfWork.SetTenantId(2))
            {
                var entity = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.EmailAddress == email);
                if (entity != null)
                {
                    var wx_id = (await _userLoginRepository.FirstOrDefaultAsync(x => x.UserId == entity.Id)).ProviderKey;
                    var mail_entity = new QYEmail.QYMailUserInfocsForUpdate
                    {
                        userid = entity.EmailAddress,
                        extid = wx_id,//entity.Id.ToString(),
                        //department = new List<long>() { 6786316460997750890 },
                        //position = entity.Position,
                        //gender = entity.Sex ? "1" : "2",
                        //mobile = entity.PhoneNumber,
                        //name = entity.Name,
                    };
                    _qyEmailManager.ResetQYEmailPassword(mail_entity, 2, "update");
                }
            }
        }

        #endregion  

        #region  Synchronize Department


        /// <summary>
        /// 同步企业邮箱 部门 to 组织
        /// </summary>
        /// <returns></returns>
        public async Task<MatchResultDto> MatchMailDepartment()
        {
            MatchResultDto result = new MatchResultDto { CreateCount = 0, DeleteCount = 0, MatchCount = 0 };
            var current_tenantid = this.AbpSession.TenantId;

            var mail_departments = _qyEmailManager.GetEmailAllDepartment(AbpSession.TenantId.Value);

            var newruzhi = mail_departments.FirstOrDefault(x => x.name == "企业微信入职人员");//6786316460997752257

            if (mail_departments != null)
            {
                var local_dept = await _organizationUnitRepository.GetAll().ToListAsync();
                foreach (var item in local_dept)
                {
                    if (item.QYMailDeptId.HasValue)
                    {
                        if (!mail_departments.Any(x => x.id == item.QYMailDeptId))
                        {

                        }
                    }
                    else
                    {
                        if (item.WXParentDeptId == 0)
                        {
                            var mail_dept = mail_departments.FirstOrDefault(x => x.name == "广东采联采购科技有限公司");
                            item.QYMailDeptId = mail_dept.id;
                        }
                        else
                        {
                            var mail_dept = mail_departments.FirstOrDefault(x => x.name == item.DisplayName);
                            if (mail_dept != null)
                            {
                                item.QYMailDeptId = mail_dept.id;
                                await _organizationUnitRepository.UpdateAsync(item);
                            }
                        }

                    }
                }
            }

            return result;
        }

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
            string str_login_provider = "Wechat";
            var relations = await _weChatManager.GetAllUsersDeptRelation();
            if (relations != null)
            {
                var current_tenantid = this.AbpSession.TenantId;
                var query_list = (from ul in _userLoginRepository.GetAll()
                                  join r in relations.AsQueryable() on new { key = ul.ProviderKey, type = ul.LoginProvider } equals new { key = r.Item1, type = str_login_provider }
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
            if (left_query.Any() && right_query.Any())
            {
                var full_query = left_query.Union(right_query);
                return full_query.ToList();
            }
            else if (left_query.Any())
            {
                return left_query.ToList();
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
        /// 通过企业微信用户ID 直接创建abp用户
        /// </summary>
        /// <param name="wechatUid"></param>
        /// <returns></returns>
        public async Task CreateUserByWechatUserId(string wechatUid)
        {
            if (!string.IsNullOrEmpty(wechatUid))
            {
                var current_tenantid = this.AbpSession.TenantId;
                var wechat_user = await _weChatManager.GetUserById(wechatUid);
                if (wechat_user != null)
                {

                    var user_id = await this.AuthCreate(new CreateAuthUserDto()
                    {
                        EmailAddress = wechat_user.email,
                        Name = wechat_user.name,
                        PhoneNumber = wechat_user.mobile,
                        Avatar = wechat_user.avatar,
                        IsActive = true,
                        Position = wechat_user.position,
                        Sex = wechat_user.gender == "1" ? true : false,
                        Surname = wechat_user.alias,
                        UserName = wechat_user.email.Replace("@chinapsp.cn", ""),
                        Password = "000000"
                    });

                    if (user_id != 0)
                    {
                        await _baseUserRepository.InsertAsync(new BaseUserEmp
                        {
                            AbpUserId = user_id,
                            EmpOrderNo = user_id.ToString(),
                            EmpStationId = "",
                            EmpStatus = "1",
                            EmpUserGuid = Guid.NewGuid().ToString("N"),
                            IsLeader = "",
                            EmpUserId = wechat_user.userid
                        });

                        await _userLoginRepository.InsertAsync(new UserLogin
                        {
                            UserId = user_id,
                            LoginProvider = "Wechat",
                            ProviderKey = wechat_user.userid,
                            TenantId = AbpSession.TenantId
                        });

                    }
                }
            }
        }

        /// <summary>
        /// 按姓名同步企业微信与base user 的用户内容级组织关联(PS:暂时无法同步角色)
        /// </summary>
        /// <param name="AbpUserId"></param>
        /// <returns></returns>
        public async Task SynchronizeUserByUserId(long AbpUserId)
        {
            var current_tenantid = this.AbpSession.TenantId;
            var ul = _userLoginRepository.FirstOrDefault(x => x.UserId == AbpUserId && x.TenantId == current_tenantid && x.LoginProvider == "Wechat");
            if (ul != null)
            {
                var i = 0;
                var wechat_user = await _weChatManager.GetUserById(ul.ProviderKey);

                var entity = await UserManager.GetUserByIdAsync(AbpUserId);
                entity.EmailAddress = wechat_user.email;
                entity.Surname = wechat_user.alias;
                entity.Avatar = wechat_user.avatar;
                entity.Position = wechat_user.position;
                entity.Name = wechat_user.name;
                await UserManager.UpdateAsync(entity);

                var base_user_emp = _baseUserRepository.GetAll().FirstOrDefault(x => x.AbpUserId == ul.UserId);
                if (base_user_emp == null)
                {
                    base_user_emp = new BaseUserEmp
                    {
                        AbpUserId = AbpUserId,
                        EmpOrderNo = ul.ProviderKey,
                        EmpStationId = Guid.NewGuid().ToString("N"),
                        EmpStatus = "1",
                        EmpUserGuid = Guid.NewGuid().ToString("N"),
                        IsLeader = "",
                        EmpUserId = ul.ProviderKey
                    };
                    base_user_emp.Id = _baseUserRepository.InsertAndGetId(base_user_emp);
                }
                if (wechat_user != null)
                {
                    var org_list = _organizationUnitRepository.GetAll().Where(o => _userOrganizationUnitRepository.GetAll().Any(x => o.Id == x.OrganizationUnitId && x.UserId == AbpUserId && x.TenantId == current_tenantid)).ToList();
                    var bs_org_list = await _baseUserManager.GetBaseUserOrgByAbpId(AbpUserId);
                    foreach (var wx_dept in wechat_user.department)
                    {
                        if (org_list.Count > 0)
                        {
                            var org_entity = _organizationUnitRepository.FirstOrDefault(x => x.WXDeptId == wx_dept);
                            if (org_entity != null)
                            {
                                await _userOrganizationUnitRepository.InsertAsync(new UserOrganizationUnit
                                {
                                    OrganizationUnitId = org_entity.Id,
                                    UserId = AbpUserId,
                                    TenantId = current_tenantid
                                });
                            }
                        }
                        else if (!org_list.Any(x => x.WXDeptId == wx_dept))
                        {
                            var org_entity = _organizationUnitRepository.FirstOrDefault(x => x.WXDeptId == wx_dept);
                            if (org_entity != null)
                            {
                                await _userOrganizationUnitRepository.InsertAsync(new UserOrganizationUnit
                                {
                                    OrganizationUnitId = org_entity.Id,
                                    UserId = AbpUserId,
                                    TenantId = current_tenantid
                                });
                            }
                        }
                        if (bs_org_list == null)
                        {
                            var bs_org_entity = await _baseUserManager.GetBaseUserOrgByWxId(wx_dept);
                            if (bs_org_entity != null)
                            {
                                _baseEmpOrgRepository.Insert(new BaseUserEmpOrg
                                {
                                    AbpUserId = AbpUserId,
                                    BaseUserGuid = Guid.NewGuid().ToString(),
                                    EmpUserGuid = base_user_emp.EmpUserGuid,
                                    CropId = "wx003757ee144cae06",
                                    DepartmentGuid = bs_org_entity.OrgGuid,
                                    EmpUserId = ul.ProviderKey,
                                    DepartmentId = bs_org_entity.Id.ToString(),
                                    IsMaster = i == 0 ? "1" : "0",
                                });
                            }
                        }
                        else if (!bs_org_list.Any(x => x.WxId == wx_dept.ToString()))
                        {
                            var bs_org_entity = await _baseUserManager.GetBaseUserOrgByWxId(wx_dept);
                            if (bs_org_entity != null)
                            {
                                _baseEmpOrgRepository.Insert(new BaseUserEmpOrg
                                {
                                    AbpUserId = AbpUserId,
                                    BaseUserGuid = Guid.NewGuid().ToString(),
                                    EmpUserGuid = base_user_emp.EmpUserGuid,
                                    CropId = "wx003757ee144cae06",
                                    DepartmentGuid = bs_org_entity.OrgGuid,
                                    EmpUserId = ul.ProviderKey,
                                    DepartmentId = bs_org_entity.Id.ToString(),
                                    IsMaster = i == 0 ? "1" : "0",
                                });
                            }
                        }
                        i++;
                    }
                }
            }
            else
            {
                throw new UserFriendlyException("无法同步没有企业微信记录的数据");
            }
        }

        /// <summary>
        /// 根据姓名获取企业微信用户
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<List<GetWxUserDto>> GetWecharUserByName(string name)
        {
            var current_tenantid = this.AbpSession.TenantId;
            //企业微信用户list
            var elist = await _weChatManager.GetAllUsersByDepartment(1);
            var query = elist.Where(x => x.name.Contains(name));

            var result = query.ToList().MapTo<List<GetWxUserDto>>();

            var ul_query = _userLoginRepository.GetAll().Where(x => x.TenantId == current_tenantid && x.LoginProvider == "Wechat" && result.Any(y => y.userid == x.ProviderKey)).ToList();

            foreach (var item in result)
            {
                var ul = ul_query.FirstOrDefault(x => x.ProviderKey == item.userid);
                if (ul != null)
                {
                    var user = _userRepository.GetAllIncluding(x => x.Roles).FirstOrDefault(x => x.Id == ul.UserId);
                    item.AbpUser = user.MapTo<UserDto>();
                    if (user.Roles.Any())
                    {
                        item.AbpUser.RoleNames = RoleManager.Roles.Where(r => user.Roles.Any(ur => ur.RoleId == r.Id)).Select(r => r.NormalizedName).ToArray();
                    }

                    item.BaseUserEmp = _baseUserRepository.GetAll().FirstOrDefault(x => x.AbpUserId == ul.UserId).MapTo<BaseUserEmpDto>();
                    var bs_role_list = await _baseUserManager.GetBaseUserRoleByAbpId(ul.UserId);
                    item.BaseUserRole = bs_role_list.MapTo<List<BaseUserRoleDto>>();
                    var bs_org_list = await _baseUserManager.GetBaseUserOrgByAbpId(ul.UserId);
                    item.BaseUserOrg = bs_org_list.MapTo<List<BaseUserDeptDto>>();
                    if (item.BaseUserOrg != null)
                    {
                        var master_id = await _baseUserManager.GetMasterIdByAbpId(ul.UserId);
                        if (master_id != 0)
                        {
                            item.BaseUserOrg.ForEach(buo =>
                            {
                                if (buo.Id == master_id)
                                {
                                    buo.IsMaster = true;
                                }
                            });
                        }
                    }

                }
            }
            return result;
        }

        /// <summary>
        /// 再次尝试获取企业微信用户与本地用户full join
        /// </summary>
        /// <param name="wx_users"></param>
        /// <returns></returns>
        private List<AbpWxUserDto> GetWxAbpUsers(ICollection<AbpWeChatUser> wx_users)
        {
            var left_query = from u in _userRepository.GetAll()
                             join ulm in _userLoginRepository.GetAll() on u.Id equals ulm.UserId into ulo
                             from ul in ulo.DefaultIfEmpty()
                             join wum in wx_users.AsQueryable() on u.EmailAddress equals wum.email into wuo
                             from wu in wuo.DefaultIfEmpty()
                             where u.TenantId == AbpSession.TenantId
                             select new AbpWxUserDto
                             {
                                 AbpUserId = u.Id,
                                 AbpRelationId = (ul == null ? 0 : ul.Id),
                                 AbpUserName = u.UserName,
                                 wx_alias = (wu == null ? "" : wu.alias),
                                 wx_avatar = (wu == null ? "" : wu.avatar),
                                 wx_email = (wu == null ? u.EmailAddress : wu.email),
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
                                  AbpUserName = wu.email.Replace("@chinapsp.cn", ""), //.Substring(0, wu.email.IndexOf('@')),
                                  AbpRelationId = 0,
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
            //var left_list = left_query.ToList();
            //var right_list = right_query.ToList();
            if (left_query.Any() && right_query.Any())
            {
                var full_query = left_query.Union(right_query);
                return full_query.ToList();
            }
            else if (left_query.Any())
            {
                return left_query.ToList();
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
            try
            {
                MatchResultDto result = new MatchResultDto { CreateCount = 0, MatchCount = 0, DeleteCount = 0 };

                var top_dept = await _organizationUnitRepository.FirstOrDefaultAsync(x => x.WXParentDeptId == 0);
                if (top_dept != null)
                {
                    //企业微信用户list
                    var elist = await _weChatManager.GetAllUsersByDepartment(1);
                    if (elist != null)
                    {
                        //var test = elist.Where(x => x.userid == "116").FirstOrDefault();
                        var full_list = this.GetWxAbpUsers(elist);
                        if (full_list != null)
                        {
                            //var test2 = full_list.Where(x => x.AbpUserName == "lidi").FirstOrDefault();
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
                                        entity.Avatar = item.wx_avatar;
                                        entity.Position = item.wx_position;
                                        entity.Name = item.wx_name;
                                        await UserManager.UpdateAsync(entity);

                                        if (item.AbpRelationId == 0)
                                        {
                                            await _userLoginRepository.InsertAsync(new UserLogin
                                            {
                                                UserId = item.AbpUserId.Value,
                                                LoginProvider = "Wechat",
                                                ProviderKey = item.wx_userid,
                                                TenantId = AbpSession.TenantId
                                            });
                                            result.MatchCount++;
                                        }
                                    }
                                }
                                else
                                {
                                    var user_id = await this.AuthCreate(new CreateAuthUserDto()
                                    {
                                        EmailAddress = item.wx_email,
                                        Name = item.wx_name,
                                        PhoneNumber = item.wx_mobile,
                                        Avatar = item.wx_avatar,
                                        IsActive = true,
                                        Position = item.wx_position,
                                        Sex = item.wx_gender == "1" ? true : false,
                                        Surname = item.wx_alias,
                                        UserName = item.AbpUserName,
                                        Password = "000000"
                                    });
                                    result.CreateCount++;

                                    if (user_id != 0)
                                    {
                                        await _baseUserRepository.InsertAsync(new BaseUserEmp
                                        {
                                            AbpUserId = user_id,
                                            EmpOrderNo = user_id.ToString(),
                                            EmpStationId = "",
                                            EmpStatus = "1",
                                            EmpUserGuid = Guid.NewGuid().ToString("N"),
                                            IsLeader = "",
                                            EmpUserId = item.wx_userid
                                        });

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
                            }
                            CurrentUnitOfWork.SaveChanges();
                        }
                    }
                }
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


            CurrentUnitOfWork.SaveChanges();

            return user.Id;

        }

        #endregion

        #region Synchronize Role

        /// <summary>
        /// 尝试获取企业微信标签与本地权限full join
        /// </summary>
        /// <param name="wechatTags"></param>
        /// <param name="tenant_id"></param>
        /// <returns></returns>
        private List<AbpWxTagRoleDto> GetAbpWechatTagRole(ICollection<AbpWeChatTag> wx_tags)
        {
            var left_query = from r in _roleRepository.GetAll()
                                 //join tm in wx_tags.AsQueryable() on new { id = r.TagId, name = r.DisplayName } equals new { id = tm.tagid, name = "WX_" + tm.tagname } into tori
                             join tm in wx_tags.AsQueryable() on r.DisplayName equals "WX_" + tm.tagname into tori
                             from t in tori.DefaultIfEmpty()
                             where r.TenantId == AbpSession.TenantId && r.WxTagId.HasValue && r.WxTagId != 0
                             select new AbpWxTagRoleDto
                             {
                                 RoleId = r.Id,
                                 RoleName = r.DisplayName,
                                 TagName = (t == null ? "" : t.tagname),
                                 WechatTagId = (t == null ? 0 : t.tagid),
                             };

            var right_query = from t in wx_tags.AsQueryable()
                                  //where !_roleRepository.GetAll().Any(r => r.TagId == t.tagid && r.DisplayName == "WX_" + t.tagname && r.TenantId == AbpSession.TenantId)
                              where !_roleRepository.GetAll().Any(r => r.DisplayName == "WX_" + t.tagname && r.TenantId == AbpSession.TenantId)
                              select new AbpWxTagRoleDto
                              {
                                  RoleId = 0,
                                  RoleName = "WX_" + t.tagname,
                                  TagName = t.tagname,
                                  WechatTagId = t.tagid
                              };
            //var tt = left_query.ToList();
            //var rr = right_query.ToList();
            if (left_query.Any() && right_query.Any())
            {
                var full_query = left_query.Union(right_query);
                return full_query.ToList();
            }
            else if (left_query.Any())
            {
                return left_query.ToList();
            }
            else if (right_query.Any())
            {
                return right_query.ToList();
            }
            return null;
        }

        /// <summary>
        /// 根据企业微信标签导入用户
        /// </summary>
        /// <returns></returns>
        public async Task<MatchResultDto> MatchTag()
        {
            MatchResultDto result = new MatchResultDto { CreateCount = 0, DeleteCount = 0, MatchCount = 0 };
            var wx_tags = await _weChatManager.GetAllTag();
            if (wx_tags != null)
            {
                var current_tenantid = this.AbpSession.TenantId;
                var local_tags = this.GetAbpWechatTagRole(wx_tags);
                if (local_tags != null)
                {
                    foreach (var item in local_tags)
                    {
                        if (item.RoleId != 0)
                        {
                            var entity = await RoleManager.GetRoleByIdAsync(item.RoleId.Value);
                            //判断是否拥有企业微信记录,否则删除本地记录
                            if (item.WechatTagId != 0)
                            {
                                entity.NormalizedName = item.TagName;
                                entity.DisplayName = item.RoleName;
                                entity.Name = item.RoleName;
                                entity.WxTagId = item.WechatTagId;
                                await RoleManager.UpdateAsync(entity);
                                result.MatchCount++;
                            }
                            else
                            {
                                await RoleManager.DeleteAsync(entity);
                                result.DeleteCount++;
                            }
                        }
                        else
                        {
                            await RoleManager.CreateAsync(new Role
                            {
                                WxTagId = item.WechatTagId,
                                IsDefault = false,
                                NormalizedName = item.TagName,
                                DisplayName = item.RoleName,
                                Name = item.RoleName,
                                TenantId = current_tenantid
                            });
                            result.CreateCount++;
                        }
                        //提交增删改记录
                        CurrentUnitOfWork.SaveChanges();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 根据企业微信标签 建立 用户 关联
        /// </summary>
        /// <returns></returns>
        public async Task<MatchResultDto> MatchTagUser()
        {
            MatchResultDto result = new MatchResultDto { CreateCount = 0, DeleteCount = 0, MatchCount = 0 };

            var local_roles = _roleRepository.GetAll().Where(r => r.WxTagId.HasValue && r.WxTagId != 0);

            if (local_roles.Any())
            {
                foreach (var item in local_roles)
                {
                    var tag_list = await _weChatManager.GetUserListByTag(item.WxTagId);
                    if (tag_list != null)
                    {
                        var tag_user_list = this.GetWechatTagUser(tag_list, item.Id, AbpSession.TenantId);
                        if (tag_user_list != null)
                        {
                            foreach (var tuItem in tag_user_list)
                            {
                                if (tuItem.UserRoleId != 0)
                                {
                                    if (tuItem.userid == "")
                                    {
                                        await _userRoleRepository.DeleteAsync(tuItem.UserRoleId.Value);
                                        CurrentUnitOfWork.SaveChanges();
                                        result.DeleteCount++;
                                    }
                                }
                                else
                                {
                                    var ul = await _userLoginRepository.GetAll().FirstOrDefaultAsync(x => x.ProviderKey == tuItem.userid && x.LoginProvider == "Wechat");
                                    if (ul != null)
                                    {
                                        await _userRoleRepository.InsertAsync(new UserRole
                                        {
                                            RoleId = item.Id,
                                            TenantId = AbpSession.TenantId,
                                            UserId = ul.UserId
                                        });
                                        CurrentUnitOfWork.SaveChanges();
                                        result.CreateCount++;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        private List<AbpWxTagUserDto> GetWechatTagUser(ICollection<AbpWeChatUser> qy_tag_user, int? role_id, int? tenant_id)
        {

            var left_query = from ur in _userRoleRepository.GetAll()
                             join ul in _userLoginRepository.GetAll() on
                                        new { uid = ur.UserId, type = "Wechat" } equals
                                        new { uid = ul.UserId, type = ul.LoginProvider } into ulo
                             from ul in ulo.DefaultIfEmpty()
                             join tu in qy_tag_user.AsQueryable() on ul.ProviderKey equals tu.userid into tuo
                             from tu in tuo.DefaultIfEmpty()
                             where ur.TenantId == tenant_id && ur.RoleId == role_id
                             select new AbpWxTagUserDto
                             {
                                 userid = (tu == null ? "" : tu.userid),
                                 name = (tu == null ? "" : tu.name),
                                 UserRoleId = ur.Id,
                                 AbpUserId = ur.UserId,

                             };

            var right_query = from tu in qy_tag_user.AsQueryable()
                                  //where !_roleRepository.GetAll().Any(r => r.TagId == t.tagid && r.DisplayName == "WX_" + t.tagname && r.TenantId == AbpSession.TenantId)
                              where !_userRoleRepository.GetAll().Any(ur => ur.RoleId == role_id && _userLoginRepository.GetAll().Any(ul =>
                                                                                                                                           ul.LoginProvider == "Wechat" &&
                                                                                                                                           ul.UserId == ur.UserId &&
                                                                                                                                           ul.ProviderKey == tu.userid
                                                                                                                                        ))
                              select new AbpWxTagUserDto
                              {
                                  name = tu.name,
                                  userid = tu.userid,
                                  UserRoleId = 0,
                                  AbpUserId = 0,
                              };
            if (left_query.Any() && right_query.Any())
            {
                var full_query = left_query.Union(right_query);
                return full_query.ToList();
            }
            else if (left_query.Any())
            {
                return left_query.ToList();
            }
            else if (right_query.Any())
            {
                return right_query.ToList();
            }
            return null;
        }

        #endregion


    }
}
