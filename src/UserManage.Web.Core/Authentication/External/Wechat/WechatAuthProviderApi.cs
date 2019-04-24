using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Abp.Runtime.Caching;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

using Senparc.Weixin;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.Work.AdvancedAPIs.OAuth2;
using Senparc.Weixin.Work.Containers;
using UserManage.Authentication.External.Wechat.Dto;

namespace UserManage.Authentication.External.Wechat
{
    public class WechatAuthProviderApi : ExternalAuthProviderApiBase
    {
        public const string Name = "Wechat";
        private readonly ICacheManager _cacheManager;
        private readonly IExternalAuthConfiguration _externalAuthConfiguration;
        WeChatMiniProgramOptions _options;
        JSchema schema = JSchema.Parse(JsonConvert.SerializeObject(new UsersWechat()));

        public WechatAuthProviderApi(ICacheManager cacheManager, IExternalAuthConfiguration externalAuthConfiguration)
        {
            _cacheManager = cacheManager;
            _externalAuthConfiguration = externalAuthConfiguration;
            var r = externalAuthConfiguration.Providers.First(p => p.Name == Name);
            _options = new WeChatMiniProgramOptions
            {
                AppId = r.ClientId,
                Secret = r.ClientSecret
            };

        }

       


        /// <summary>
        /// 通过企业微信获取用户信息
        /// </summary>
        /// <param name="accessCode"></param>
        /// <returns></returns>
        public override async Task<ExternalAuthUserInfo> GetUserInfo(string accessCode)
        {
            try
            {
                UsersWechat wechat = new UsersWechat();

                string accessToken = await this.GetToken(_options.AppId, _options.Secret);//_cacheManager.GetCache("CacheName").Get("Login", () => GetToken(_options.AppId, _options.Secret));

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    var url = string.Format(Config.ApiWorkHost + "/cgi-bin/user/getuserinfo?access_token={0}&code={1}",
                        accessToken, accessCode);

                    var redata = Get.GetJson<GetUserInfoResult>(url);
                    if (!string.IsNullOrWhiteSpace(redata.UserId))
                    {
                        //UserTicket tiket = new UserTicket
                        //{
                        //    user_ticket = redata.user_ticket
                        //};
                        //url = string.Format(Config.ApiWorkHost + "/cgi-bin/user/getuserdetail?access_token={0}",
                        //    accessToken);
                        //   wechat = Post.GetResult<UsersWechat>(JsonConvert.SerializeObject(tiket));

                        url = string.Format(Config.ApiWorkHost + "/cgi-bin/user/get?access_token={0}&userid={1}",
                            accessToken, redata.UserId);
                        wechat = await Get.GetJsonAsync<UsersWechat>(url);
                        //wechat = await GetUserMsg(url, tiket);
                    }
                }

                var t = wechat == null
                    ? new ExternalAuthUserInfo()
                    : new ExternalAuthUserInfo
                    {
                        EmailAddress = wechat.email,
                        //Surname = wechat.name,
                        ProviderKey = wechat.userid,
                        Provider = Name,
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


        private async Task<string> GetToken(string AppId, string Secret)
        {
            try
            {

        
            if (string.IsNullOrWhiteSpace(AppId) || string.IsNullOrWhiteSpace(Secret))
            {
                return "";
            }
            string token_result = "";
            var token = await _cacheManager.GetCache("CurrentToken").GetOrDefaultAsync(AppId);
            if (token == null)
            {
                token =  AccessTokenContainer.TryGetToken(AppId, Secret);
                if (token != null)
                {
                    await _cacheManager.GetCache("CurrentToken").SetAsync(this._options.AppId, token.ToString(), TimeSpan.FromHours(2));
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

        public static Dictionary<string, string> Gets(string url, System.Text.Encoding encode = null)
        {
            try
            {
                WebClient c = new WebClient();
                byte[] data = c.DownloadData(url);
                if (encode == null)
                    encode = System.Text.Encoding.UTF8;
                string data_text = encode.GetString(data);
                // System.Collections.Generic.Dictionary < string,string>

                return JsonConvert.DeserializeObject<Dictionary<string, string>>(data_text);
                ;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

    }
}
