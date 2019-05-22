using Abp.Runtime.Caching;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UserManage.AbpExternalAuthenticateCore;
using UserManage.AbpExternalAuthenticateCore.DomainService;
using UserManage.Web;

namespace UserManage.QYEmail.DomainService
{
    public class QYEmailManager : UserManageDomainServiceBase, IQYEmailManager
    {
        private readonly IAbpExternalAuthenticateConfigManager _authenticateConfigManager;

        private readonly ICacheManager _cacheManager;

        public const string DefaultProviderName = "QYEmail";

        public QYEmailManager(
            IAbpExternalAuthenticateConfigManager authenticateConfigManager,
            ICacheManager cacheManager)
        {
            _authenticateConfigManager = authenticateConfigManager;
            _cacheManager = cacheManager;
        }

        // TODO:编写领域业务代码


        public string Test(int tenant_id)
        {
            AbpExternalAuthenticateConfig auth_config = this.GetCurrentAuthWithoutTenant(tenant_id);
            string acc_token = this.GetToken(auth_config.AppId, auth_config.Secret);
            return acc_token;
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
            string token_result = "";
            var token = _cacheManager.GetCache(UserManageConsts.Abp_QYEmail_Access_Token_Cache)
                .Get(appId, () =>
                        this.getToken(appId, secret));
            token_result = token?.ToString();

            return token_result;
        }

        /// <summary>
        /// 获取当前秘钥的token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        private string getToken(string appId, string secret)
        {
            if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(secret))
            {
                return "";
            }
            string url = string.Format(@"https://api.exmail.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}", appId, secret);
            return HttpMethods.RestGet(url);
        }

        #endregion
    }
}
