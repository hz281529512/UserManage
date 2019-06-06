using Abp.Runtime.Caching;
using Senparc.Weixin;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.Work.AdvancedAPIs.OAuth2;
using Senparc.Weixin.Work.Containers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UserManage.AbpServiceCore.DomainService;
using UserManage.Authentication.External.Wechat.Dto;

namespace UserManage.Authentication.External.Wechat
{
    public class WechatAuthProviderManager
    {
        private readonly IAbpServiceConfigManager  _configManager;
        //缓存
        private readonly ICacheManager _cacheManager;

        public WechatAuthProviderManager(
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
        public  async Task<ExternalAuthUserInfo> GetUserInfo(string local_name,string accessCode)
        {
            try
            {
                var _config = await _configManager.GetCurrentConfigAsync(local_name);
                if (_config == null) throw new Abp.UI.UserFriendlyException("未能找到对应APP配置 : " + local_name);


                UsersWechat wechat = new UsersWechat();

                string accessToken = await this.GetToken(_config.LocalName,_config.CropId, _config.Secret);//_cacheManager.GetCache("CacheName").Get("Login", () => GetToken(_options.CropId, _options.Secret));

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
                    ? new ExternalAuthUserInfo()
                    : new ExternalAuthUserInfo
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


        private async Task<string> GetToken(string local_name,string CropId, string Secret)
        {
            try
            {
                

                if (string.IsNullOrWhiteSpace(CropId) || string.IsNullOrWhiteSpace(Secret))
                {
                    return "";
                }
                string token_result = "";
                var token = await _cacheManager.GetCache(UserManageConsts.Abp_Login_Token_Cache).GetOrDefaultAsync(CropId);
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
    }
}
