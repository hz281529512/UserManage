
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.UI;
using Abp.Runtime.Caching;
using UserManage.AbpExternalAuthenticateCore.DomainService;
using Senparc.Weixin;
using Senparc.Weixin.HttpUtility;
using UserManage.AbpExternalCore.Model;
using UserManage.AbpExternalAuthenticateCore;

namespace UserManage.AbpExternalCore.DomainService
{
    /// <summary>
    /// AbpWeChat领域层的业务管理
    ///</summary>
    public class AbpWeChatManager : UserManageDomainServiceBase, IAbpWeChatManager
    {
        private readonly IAbpExternalAuthenticateConfigManager _authenticateConfigManager;

        private readonly ICacheManager _cacheManager;

        public const string DefaultProviderName = "Wechat";

        public AbpWeChatManager(
            IAbpExternalAuthenticateConfigManager authenticateConfigManager,
            ICacheManager cacheManager
        )
        {
            _authenticateConfigManager = authenticateConfigManager;
            _cacheManager = cacheManager;
        }

        // TODO:编写领域业务代码

        // TODO:编写领域业务代码

        #region Private
            
        private async Task<ICollection<AbpWeChatDepartment>> GetAllDepartments(string accessToken)
        {
            ICollection<AbpWeChatDepartment> result = null;
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                var url = string.Format(Config.ApiWorkHost + "/cgi-bin/department/list?access_token={0}",
                               accessToken);

                var rdata = await Get.GetJsonAsync<AbpDepartmentResult>(url);
                if (rdata.department?.Count > 0)
                {
                    result = rdata.department;
                }
            }
            return result;
        }


        private async Task<ICollection<AbpWeChatDepartment>> GetDepartmentsById(string accessToken, int? parent_id = null, bool is_only_one_layer = false)
        {
            try
            {
                ICollection<AbpWeChatDepartment> result = null;

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    var url = string.Format(Config.ApiWorkHost + "/cgi-bin/department/list?access_token={0}&id={1}",
                                   accessToken, parent_id ?? 1);

                    var rdata = await Get.GetJsonAsync<AbpDepartmentResult>(url);
                    if (rdata.department?.Count > 0)
                    {
                        result = (from d in rdata.department
                                  where (!is_only_one_layer || d.parentid == (parent_id ?? 1))
                                  select d).ToList();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// 通过部门ID 获取微信用户与部门关联信息
        /// </summary>
        /// <param name="parent_id"></param>
        /// <returns></returns>
        private async Task<ICollection<AbpWeChatUserDepartment>> GetUserRelationByDeptId(string accessToken, int? parent_id = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    //参数	必须	说明
                    //access_token 是   调用接口凭证
                    //department_id   是 获取的部门id
                    //fetch_child 否   1 / 0：是否递归获取子部门下面的成员
                    var url = string.Format(Config.ApiWorkHost + "/cgi-bin/user/simplelist?access_token={0}&department_id={1}&fetch_child={2}",
                      accessToken, parent_id, 1);
                    var redata = await Get.GetJsonAsync<AbpUserDepartmentResult>(url);
                    if (redata.errcode == "0")
                    {
                        if (redata.userlist?.Count > 0)
                        {
                            return redata.userlist;
                        }
                    }
                    else
                    {
                        Console.WriteLine(redata.errmsg);
                    }
                }
                return null;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// 通过部门ID 获取微信用户信息
        /// </summary>
        /// <param name="parent_id"></param>
        /// <returns></returns>
        private async Task<ICollection<AbpWeChatUser>> GetUserByDeptId(string accessToken, int? parent_id = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    //参数	必须	说明
                    //access_token 是   调用接口凭证
                    //department_id   是 获取的部门id
                    //fetch_child 否   1 / 0：是否递归获取子部门下面的成员
                    //status  否   0获取全部成员，1获取已关注成员列表，2获取禁用成员列表，4获取未关注成员列表。status可叠加,未填写则默认为4
                    var url = string.Format(Config.ApiWorkHost + "/cgi-bin/user/list?access_token={0}&department_id={1}&fetch_child={2}",
                      accessToken, parent_id, 1);
                    var redata = await Get.GetJsonAsync<AbpUserResult>(url);
                    if (redata.errcode == "0")
                    {
                        if (redata.userlist?.Count > 0)
                        {
                            return redata.userlist;
                        }
                    }
                    else
                    {
                        Console.WriteLine(redata.errmsg);
                    }
                }
                return null;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// 获取所有标签
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private async Task<ICollection<AbpWeChatTag>> GetAllTag(string accessToken)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    //参数	必须	说明
                    //access_token 是   调用接口凭证
                    var url = string.Format(Config.ApiWorkHost + "/cgi-bin/tag/list?access_token={0}",
                      accessToken);
                    var redata = await Get.GetJsonAsync<AbpTagResult>(url);
                    if (redata.errcode == "0")
                    {
                        if (redata.taglist?.Count > 0)
                        {
                            return redata.taglist;
                        }
                    }
                    else
                    {
                        Console.WriteLine(redata.errmsg);
                    }
                }
                return null;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// 获取标签用户
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="tag_id"></param>
        /// <returns></returns>
        private async Task<ICollection<AbpWeChatUser>> GetTagUserList(string accessToken, int? tag_id)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    //参数	必须	说明
                    //access_token 是   调用接口凭证
                    //tagid   是 标签ID
                    var url = string.Format(Config.ApiWorkHost + "/cgi-bin/tag/get?access_token={0}&tagid={1}",
                      accessToken, tag_id);
                    var redata = await Get.GetJsonAsync<AbpTagUserResult>(url);
                    if (redata.errcode == "0")
                    {
                        if (redata.userlist?.Count > 0)
                        {
                            return redata.userlist;
                        }
                    }
                    else
                    {
                        Console.WriteLine(redata.errmsg);
                    }
                }
                return null;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// 获取当前秘钥的token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        private async Task<string> GetToken(string appId, string secret)
        {

            //if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(secret))
            //{
            //    return "";
            //}
            string token_result = "";
            var token = await _cacheManager.GetCache(UserManageConsts.Abp_Wechat_Access_Token_Cache)
                .GetAsync(appId, () =>
                        Senparc.Weixin.Work.Containers.AccessTokenContainer.TryGetTokenAsync(appId, secret));
            //if (token == null)
            //{
            //    token = await AccessTokenContainer.TryGetTokenAsync(AppId, Secret);
            //    if (token != null)
            //    {
            //        await _cacheManager.GetCache("CurrentToken").SetAsync(AppId, token.ToString(), TimeSpan.FromHours(2));
            //    }
            //}
            token_result = token?.ToString();

            return token_result;
        }

        /// <summary>
        /// 获取当前appid与密钥
        /// </summary>
        /// <returns></returns>
        private async Task<AbpExternalAuthenticateConfig> GetCurrentAuth()
        {
            AbpExternalAuthenticateConfig auth_config = await _authenticateConfigManager.GetCurrentAuth(DefaultProviderName);
            if (auth_config == null) throw new UserFriendlyException("外部链接配置缺失");
            return auth_config;
        }

        #endregion

        /// <summary>
        /// 根据Dept ID 获取所有Users
        /// </summary>
        /// <param name="parent_id"></param>
        /// <returns></returns>
        public async Task<ICollection<AbpWeChatUser>> GetAllUsersByDepartment(int? parent_id)
        {
            AbpExternalAuthenticateConfig auth_config = await this.GetCurrentAuth();
            string acc_token = await this.GetToken(auth_config.AppId, auth_config.Secret);

            return await this.GetUserByDeptId(acc_token, parent_id);
        }

        /// <summary>
        /// 根据Dept ID 获取所有用户部门对照数据
        /// </summary>
        /// <param name="parent_id"></param>
        /// <returns></returns>
        public async Task<ICollection<Tuple<string, int>>> GetAllUsersDeptRelation()
        {
            AbpExternalAuthenticateConfig auth_config = await this.GetCurrentAuth();
            string acc_token = await this.GetToken(auth_config.AppId, auth_config.Secret);
            var relations = await GetUserRelationByDeptId(acc_token, 1);
            if (relations != null)
            {
                var result = new List<Tuple<string, int>>();
                relations.ToList().ForEach(x =>
                {
                    foreach (var item in x.department)
                    {
                        result.Add(new Tuple<string, int>(x.userid, item));
                    }
                });
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取所有部门
        /// </summary>
        public async Task<ICollection<AbpWeChatDepartment>> GetAllDepartments()
        {
            AbpExternalAuthenticateConfig auth_config = await this.GetCurrentAuth();
            string acc_token = await this.GetToken(auth_config.AppId, auth_config.Secret);
            return await this.GetAllDepartments(acc_token);
        }

        /// <summary>
        /// 获取企业微信所有标签
        /// </summary>
        /// <returns></returns>
        public async Task<ICollection<AbpWeChatTag>> GetAllTag()
        {
            AbpExternalAuthenticateConfig auth_config = await this.GetCurrentAuth();
            string acc_token = await this.GetToken(auth_config.AppId, auth_config.Secret);
            return await this.GetAllTag(acc_token);
        }

        /// <summary>
        /// 根据Dept ID 获取所有Users
        /// </summary>
        /// <param name="wechat_tag_id"></param>
        /// <returns></returns>
        public async Task<ICollection<AbpWeChatUser>> GetUserListByTag(int? wechat_tag_id)
        {
            AbpExternalAuthenticateConfig auth_config = await this.GetCurrentAuth();
            string acc_token = await this.GetToken(auth_config.AppId, auth_config.Secret);
            return await GetTagUserList(acc_token, wechat_tag_id);
        }

        /// <summary>
        /// 根据父节点ID获取Department
        /// </summary>
        public async Task<ICollection<AbpWeChatDepartment>> GetDepartmentsById(int? parent_id = null, bool is_only_one_layer = false)
        {
            AbpExternalAuthenticateConfig auth_config = await this.GetCurrentAuth();
            string acc_token = await this.GetToken(auth_config.AppId, auth_config.Secret);
            return await this.GetDepartmentsById(acc_token, parent_id, is_only_one_layer);
        }
    }
}
