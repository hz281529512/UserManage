using Abp.Runtime.Caching;
using Abp.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManage.AbpExternalAuthenticateCore;
using UserManage.AbpExternalAuthenticateCore.DomainService;
using UserManage.AbpExternalCore;
using UserManage.AbpExternalCore.Model;
using UserManage.Web;

namespace UserManage.QYEmail.DomainService
{
    public class QYEmailManager : UserManageDomainServiceBase, IQYEmailManager
    {
        private readonly IAbpExternalAuthenticateConfigManager _authenticateConfigManager;

        private readonly ICacheManager _cacheManager;

        public const string DefaultProviderName = "QYmail";

        public QYEmailManager(
            IAbpExternalAuthenticateConfigManager authenticateConfigManager,
            ICacheManager cacheManager)
        {
            _authenticateConfigManager = authenticateConfigManager;
            _cacheManager = cacheManager;
        }

        // TODO:编写领域业务代码


        public List<QYMailDepartment> Test(int tenant_id)
        {
            return this.GetEmailAllDepartment(tenant_id);
        }


        public List<QYMailDepartment> GetEmailAllDepartment(int tenant_id)
        {
            AbpExternalAuthenticateConfig auth_config = this.GetCurrentAuthWithoutTenant(tenant_id);
            string acc_token = this.GetToken(auth_config.AppId, auth_config.Secret);
            string url = string.Format(@"https://api.exmail.qq.com/cgi-bin/department/list?access_token={0}", acc_token);

            var content = HttpMethods.RestGet(url);
            var result = JsonConvert.DeserializeObject<QYMailDepartmentResult>(content);
            if (result.errmsg == "ok")
            {
                return result.department;
            }
            else
            {
                return null;
            }
        }

        public List<QYMailCheckUserDtl> CheckMailUser(List<string> userList)
        {
            AbpExternalAuthenticateConfig auth_config = this.GetCurrentAuthWithoutTenant(2);
            string acc_token = this.GetToken(auth_config.AppId, auth_config.Secret);
            string url = string.Format(@"https://api.exmail.qq.com/cgi-bin/user/batchcheck?access_token={0}", acc_token);
            var result = new List<QYMailCheckUserDtl>();
            //var param = new QYMailCheckUserSearch();
            Dictionary<string, object> param = new Dictionary<string, object>();
            var param_list = new List<string>();
            var i = 0;
            foreach (var item in userList)
            {
                param_list.Add(item);
                if (param_list.Count == 20)
                {
                    param.Add("userlist", param_list);
                    var content1 = HttpMethods.RestJsonPost(url, param);
                    var m1 = JsonConvert.DeserializeObject<QYMailCheckUser>(content1);
                    if (m1.errmsg == "ok")
                    {
                        result = result.Concat(m1.list).ToList();
                    }
                    param_list.Clear();
                    param.Clear();
                }
            }
            param.Add("userlist", param_list);
            var content = HttpMethods.RestJsonPost(url, param);
            var m = JsonConvert.DeserializeObject<QYMailCheckUser>(content);
            if (m.errmsg == "ok")
            {
                result = result.Concat(m.list).ToList();
            }

            return result;
        }

        

        public QYMailUserInfocsForSeach GetUserInfo(int tenant_id, string email)
        {
            AbpExternalAuthenticateConfig auth_config = this.GetCurrentAuthWithoutTenant(tenant_id);
            string acc_token = this.GetToken(auth_config.AppId, auth_config.Secret);
            string url = string.Format(@"https://api.exmail.qq.com/cgi-bin/user/get?access_token={0}&userid={1}", acc_token, email);

            var content = HttpMethods.RestGet(url);
            var result = JsonConvert.DeserializeObject<QYMailUserInfocsForSeach>(content);
            if (result.errmsg == "ok")
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public void UpdateQYEmail(QYMailUserInfocsForUpdate model, int tenant_id, string change_type)
        {
            AbpExternalAuthenticateConfig auth_config = this.GetCurrentAuthWithoutTenant(tenant_id);
            model.access_token = this.GetToken(auth_config.AppId, auth_config.Secret);

            string url = change_type == "create" ? @"https://api.exmail.qq.com/cgi-bin/user/create" : @"https://api.exmail.qq.com/cgi-bin/user/update";
            url = url + string.Format(@"?access_token={0}", model.access_token);

            Dictionary<string, object> param = new Dictionary<string, object>();
            //param.Add("access_token", model.access_token);
            param.Add("department", model.department);

            //param.Add("extid", model.extid);
            param.Add("gender", model.gender);
            param.Add("mobile", model.mobile);
            param.Add("position", model.position);
            param.Add("name", model.name);
            param.Add("userid", model.userid);
            if (change_type == "create")
            {
                param.Add("password", model.extid + "Cailian");
            }
            else
            {
                param.Add("password", model.extid + "Cailian");
                //param.Add("enable", model.enable);
            }
            var p = JsonConvert.SerializeObject(param);
            var content = HttpMethods.RestJsonPost(url, param);

        }

        #region Private

        /// <summary>
        /// 获取当前appid与密钥
        /// </summary>
        /// <returns></returns>
        private AbpExternalAuthenticateConfig GetCurrentAuthWithoutTenant(int tenant_id)
        {
            AbpExternalAuthenticateConfig auth_config = _authenticateConfigManager.GetCurrentAuth(DefaultProviderName, tenant_id);
            if (auth_config == null) throw new UserFriendlyException("外部链接配置缺失");
            return auth_config;
        }


        /// <summary>
        /// 获取当前秘钥的token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        private string GetToken(string appId, string secret)
        {
            //_cacheManager.GetCache(UserManageConsts.Abp_QYEmail_Access_Token_Cache).Clear();
            var token = _cacheManager.GetCache(UserManageConsts.Abp_QYEmail_Access_Token_Cache).GetOrDefault(appId);

            if (token == null)
            {
                var token_result = this.getToken(appId, secret);
                if (token_result == null) return "";

                _cacheManager.GetCache(UserManageConsts.Abp_QYEmail_Access_Token_Cache).Set(appId, token_result.access_token, TimeSpan.FromSeconds(token_result.expires_in.Value));
                return token_result.access_token;
            }
            return token as string;
        }

        /// <summary>
        /// 获取当前秘钥的token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        private QYMailAccessToken getToken(string appId, string secret)
        {
            if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(secret))
            {
                return null;
            }
            string url = string.Format(@"https://api.exmail.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}", appId, secret);
            var content = HttpMethods.RestGet(url);

            var result = JsonConvert.DeserializeObject<QYMailAccessToken>(content);
            if (result.errmsg == "ok")
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
