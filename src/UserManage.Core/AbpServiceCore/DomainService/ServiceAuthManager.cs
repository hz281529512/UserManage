using Abp.Runtime.Caching;
using Senparc.Weixin;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.Work.AdvancedAPIs.OAuth2;
using Senparc.Weixin.Work.Containers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UserManage.AbpServiceCore.DomainService
{
    /// <summary>
    /// AbpServiceConfig领域层的业务管理
    ///</summary>
    public class ServiceAuthManager : UserManageDomainServiceBase, IServiceAuthManager
    {
        private readonly IAbpServiceConfigManager _configManager;
        //缓存
        private readonly ICacheManager _cacheManager;

        public ServiceAuthManager(
            IAbpServiceConfigManager configManager,
            ICacheManager cacheManager
            )
        {
            _configManager = configManager;
            _cacheManager = cacheManager;
        }


        /// <summary>
        /// 通过企业微信获取用户信息
        /// </summary>
        /// <param name="accessCode"></param>
        /// <returns></returns>
        public async Task<AuthUserInfo> GetUserInfo(string local_name, string accessCode)
        {
            if (local_name.IndexOf("Mini") > -1)
            {
                return await this.GetMiniUserInfo(local_name, accessCode);
            }
            else
            {
                return await this.GetQYUserInfo(local_name, accessCode);
            }
        }

        #region 小程序

        /// <summary>
        /// 通过小程序获取企业微信用户信息
        /// </summary>
        /// <param name="accessCode"></param>
        /// <returns></returns>
        private async Task<AuthUserInfo> GetMiniUserInfo(string local_name, string accessCode)
        {
            var _config = await _configManager.GetCurrentConfigAsync(local_name);
            if (_config == null) throw new Abp.UI.UserFriendlyException("未能找到对应APP配置 : " + local_name);

            UsersWechat wechat = new UsersWechat();

            string accessToken = await this.GetToken(_config.LocalName, _config.CropId, _config.Secret);

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                var url = string.Format(Config.ApiWorkHost + "/cgi-bin/miniprogram/jscode2session?access_token={0}&js_code={1}&grant_type=authorization_code",
                    accessToken, accessCode);

                var redata = Get.GetJson<Code2Session>(url);
                if (!string.IsNullOrWhiteSpace(redata.userid))
                {

                    url = string.Format(Config.ApiWorkHost + "/cgi-bin/user/get?access_token={0}&userid={1}",
                        accessToken, redata.userid);
                    wechat = await Get.GetJsonAsync<UsersWechat>(url);
                }
            }

            var t = wechat == null
                    ? new AuthUserInfo()
                    : new AuthUserInfo
                    {
                        EmailAddress = wechat.email,
                        //Surname = wechat.name,
                        ProviderKey = wechat.userid,
                        Provider = _config.LoginProvider,
                        Name = wechat.name,
                        PhoneNumber = wechat.mobile
                    };
            return t;
        }


        #endregion  

        #region 企业微信

        /// <summary>
        /// 通过企业微信获取用户信息
        /// </summary>
        /// <param name="accessCode"></param>
        /// <returns></returns>
        public async Task<AuthUserInfo> GetQYUserInfo(string local_name, string accessCode)
        {
            try
            {
                var _config = await _configManager.GetCurrentConfigAsync(local_name);
                if (_config == null) throw new Abp.UI.UserFriendlyException("未能找到对应APP配置 : " + local_name);


                UsersWechat wechat = new UsersWechat();

                string accessToken = await this.GetToken(_config.LocalName, _config.CropId, _config.Secret);//_cacheManager.GetCache("CacheName").Get("Login", () => GetToken(_options.CropId, _options.Secret));

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    var url = string.Format(Config.ApiWorkHost + "/cgi-bin/user/getuserinfo?access_token={0}&code={1}",
                        accessToken, accessCode);

                    var redata = Get.GetJson<GetUserInfoResult>(url);
                    if (!string.IsNullOrWhiteSpace(redata.UserId))
                    {

                        url = string.Format(Config.ApiWorkHost + "/cgi-bin/user/get?access_token={0}&userid={1}",
                            accessToken, redata.UserId);
                        wechat = await Get.GetJsonAsync<UsersWechat>(url);
                    }
                }

                var t = wechat == null
                    ? new AuthUserInfo()
                    : new AuthUserInfo
                    {
                        EmailAddress = wechat.email,
                        //Surname = wechat.name,
                        ProviderKey = wechat.userid,
                        Provider = _config.LoginProvider,
                        Name = wechat.name,
                        PhoneNumber = wechat.mobile
                    };
                return t;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public async Task<string> GetToken(string local_name, string CropId, string Secret)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(CropId) || string.IsNullOrWhiteSpace(Secret))
                {
                    return "";
                }
                string token_result = "";
                var token = await _cacheManager.GetCache(UserManageConsts.Abp_Login_Token_Cache).GetOrDefaultAsync(local_name);
                if (token == null)
                {
                    token = AccessTokenContainer.TryGetToken(CropId, Secret);
                    if (token != null)
                    {
                        await _cacheManager.GetCache(UserManageConsts.Abp_Login_Token_Cache).SetAsync(local_name, token.ToString(), TimeSpan.FromHours(2));
                    }
                }
                token_result = token?.ToString();

                return token_result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        } 
        #endregion
    }
}
