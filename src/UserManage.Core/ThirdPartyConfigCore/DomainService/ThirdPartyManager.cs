

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Linq;
using Abp.Linq.Extensions;
using Abp.Extensions;
using Abp.UI;
using Abp.Domain.Repositories;
using Abp.Domain.Services;

using UserManage;
using UserManage.ThirdPartyConfigCore;
using Abp.Runtime.Caching;
using Newtonsoft.Json;
using System.Xml.Linq;
using Tencent;
using UserManage.Web;
using Castle.Core.Logging;

namespace UserManage.ThirdPartyConfigCore.DomainService
{
    /// <summary>
    /// ThirdPartyConfig领域层的业务管理
    ///</summary>
    public class ThirdPartyManager : UserManageDomainServiceBase, IThirdPartyManager
    {

        private readonly IRepository<ThirdPartyConfig, Guid> _repository;

        //缓存
        private readonly ICacheManager _cacheManager;

        private ILogger _logger { get; set; }

        /// <summary>
        /// ThirdPartyConfig的构造方法
        ///</summary>
        public ThirdPartyManager(
            IRepository<ThirdPartyConfig, Guid> repository,
            ICacheManager cacheManager
        )
        {
            _repository = repository;
            _cacheManager = cacheManager;
            _logger = NullLogger.Instance;
        }



        // TODO:编写领域业务代码

        public Task<string> Test(string tp_id)
        {
            var _config = this.GetConfig(tp_id);
            return this.GetSuiteToken(tp_id, _config.SuiteID, _config.Secret);
        }


        #region Config

        public ThirdPartyConfig GetConfig(string id)
        {

            //读取缓存
            var resultConfig = _cacheManager.GetCache(UserManageConsts.Third_Party_Config_Cache).GetOrDefault(id);
            if (resultConfig == null)
            {
                var auth = _repository.Get(new Guid(id));
                if (auth == null) return null;
                var str_auth = JsonConvert.SerializeObject(auth);
                _cacheManager.GetCache(UserManageConsts.Third_Party_Config_Cache).Set(id, str_auth, TimeSpan.FromHours(8));
                return auth;
            }
            else
            {
                return JsonConvert.DeserializeObject<ThirdPartyConfig>(resultConfig as string);
            }
        }
        #endregion

        #region Verify

        public string VerifyUrl(string id, string msg_signature, string timestamp, string nonce, string echostr)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var _config = this.GetConfig(id);
                string token = _config.Token;
                string aeskey = _config.EncodingAESKey;
                string corpid = _config.CropId;
                WXBizMsgCrypt wxcpt = new WXBizMsgCrypt(token, aeskey, corpid);
                int ret = 0;
                string sEchoStr = "";
                ret = wxcpt.VerifyURL(msg_signature, timestamp, nonce, echostr, ref sEchoStr);
                if (ret != 0)
                {
                    return ret.ToString();
                }
                else
                {
                    return sEchoStr;
                }
            }
            else
            {
                return "";
            }
        }


        public string VerifySuiteTicket(string id, string signature, string timestamp, string nonce, string stringInput)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var _config = this.GetConfig(id);
                if (_config != null)
                {
                    string token = _config.Token;
                    string aeskey = _config.EncodingAESKey;
                    string corpid = _config.SuiteID;
                    WXBizMsgCrypt wxcpt = new WXBizMsgCrypt(token, aeskey, corpid);

                    string sMsg = "";  //解析之后的明文
                    int ret = 0;
                    ret = wxcpt.DecryptMsg(signature, timestamp, nonce, stringInput, ref sMsg);
                    if (ret != 0)
                        return string.Format("解析错误{0}", ret);
                    _logger.Info(sMsg);
                    var xDoc = XDocument.Parse(sMsg);
                    var q = (from c in xDoc.Elements() select c).ToList();
                    var infoType = q.Elements("InfoType").First().Value;
                    switch (infoType)
                    {
                        case "suite_ticket":
                            var ComponentVerifyTicket = q.Elements("SuiteTicket").First().Value;
                            //_logger.Info(sMsg);
                            _cacheManager.GetCache(UserManageConsts.Third_Party_Ticket_Cache).Set(id, ComponentVerifyTicket, TimeSpan.FromMinutes(30));
                            //this.SetSuiteToken(corpid, _config.Secret, ComponentVerifyTicket);
                            return "success";
                        case "unauthorized":
                            return string.Format("{0} 已取消授权", q.Elements("AuthorizerAppid").First().Value);
                        default:
                            break;
                    }
                }
                //int ret = 0;
                //string sEchoStr = "";
                //ret = wxcpt.VerifyURL(msg_signature, timestamp, nonce, echostr, ref sEchoStr);
                //return sEchoStr;
            }
            return "参数错误！";

        }

        internal async Task<string> GetSuiteTicketAsync(string tpid)
        {
            var suite_ticket = await _cacheManager.GetCache(UserManageConsts.Third_Party_Ticket_Cache).GetOrDefaultAsync(tpid);
            return suite_ticket?.ToString() ?? "";
        }

        #endregion

        #region Token

        /// <summary>
        /// 获取第三方Token
        /// </summary>
        /// <param name="tpid"></param>
        /// <returns></returns>
        public async Task<string> GetSuiteToken(string tpid)
        {
            try
            {

                var _config = this.GetConfig(tpid);
                return await this.GetSuiteToken(tpid, _config.SuiteID, _config.Secret);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取第三方token
        /// </summary>
        /// <param name="tpid"></param>
        /// <returns></returns>
        private async Task<string> GetSuiteToken(string tpid, string suite_id, string suite_secret)
        {
            var suite_ticket = await GetSuiteTicketAsync(tpid);


            var token = _cacheManager.GetCache(UserManageConsts.Third_Party_Token_Cache).GetOrDefault(suite_id);

            if (token == null)
            {
                var token_result = this.getSuiteToken(suite_id, suite_secret, suite_ticket);
                if (token_result.errmsg != "ok") return "";

                await _cacheManager.GetCache(UserManageConsts.Third_Party_Token_Cache).SetAsync(suite_id, token_result.suite_access_token, TimeSpan.FromSeconds(token_result.expires_in.Value));
                return token_result.suite_access_token;
            }
            return token as string;
        }

        /// <summary>
        /// 保存第三方秘钥到缓存
        /// </summary>
        /// <param name="suite_id"></param>
        /// <param name="suite_secret"></param>
        /// <param name="suite_ticket"></param>
        private void SetSuiteToken(string suite_id, string suite_secret, string suite_ticket)
        {
            var token = _cacheManager.GetCache(UserManageConsts.Third_Party_Token_Cache).GetOrDefault(suite_id);

            if (token == null)
            {
                var token_result = this.getSuiteToken(suite_id, suite_secret, suite_ticket);
                if (token_result.errmsg != "ok")
                {
                    _logger.Info(token_result.errmsg);
                }
                else
                {
                    _cacheManager.GetCache(UserManageConsts.Third_Party_Token_Cache).SetAsync(suite_id, token_result.suite_access_token, TimeSpan.FromSeconds(token_result.expires_in.Value));
                }
            }
        }

        /// <summary>
        /// 获取当前秘钥的token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        private SuiteAccessToken getSuiteToken(string suite_id, string suite_secret, string suite_ticket)
        {
            if (string.IsNullOrWhiteSpace(suite_id) || string.IsNullOrWhiteSpace(suite_secret) || string.IsNullOrWhiteSpace(suite_ticket))
            {
                return null;
            }
            string url = @"https://qyapi.weixin.qq.com/cgi-bin/service/get_suite_token";

            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("suite_id", suite_id);

            param.Add("suite_secret", suite_secret);
            param.Add("suite_ticket", suite_ticket);

            var content = HttpMethods.RestJsonPost(url, param);

            var result = JsonConvert.DeserializeObject<SuiteAccessToken>(content);
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
