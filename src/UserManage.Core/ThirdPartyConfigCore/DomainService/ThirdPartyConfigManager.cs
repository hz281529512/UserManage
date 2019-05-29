

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

namespace UserManage.ThirdPartyConfigCore.DomainService
{
    /// <summary>
    /// ThirdPartyConfig领域层的业务管理
    ///</summary>
    public class ThirdPartyConfigManager : UserManageDomainServiceBase, IThirdPartyConfigManager
    {

        private readonly IRepository<ThirdPartyConfig, Guid> _repository;

        //缓存
        private readonly ICacheManager _cacheManager;

        /// <summary>
        /// ThirdPartyConfig的构造方法
        ///</summary>
        public ThirdPartyConfigManager(
            IRepository<ThirdPartyConfig, Guid> repository,
            ICacheManager cacheManager
        )
        {
            _repository = repository;
            _cacheManager = cacheManager;
        }


        /// <summary>
        /// 初始化
        ///</summary>
        public void InitThirdPartyConfig()
        {
            throw new NotImplementedException();
        }

        // TODO:编写领域业务代码




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

        public int DecryptContent(string id, string msg_signature, string timestamp, string nonce, string content)
        {
            throw new NotImplementedException();
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
                    var xDoc = XDocument.Parse(sMsg);
                    var q = (from c in xDoc.Elements() select c).ToList();
                    var infoType = q.Elements("InfoType").First().Value;
                    switch (infoType)
                    {
                        case "suite_ticket":
                            var ComponentVerifyTicket = q.Elements("SuiteTicket").First().Value;
                            _cacheManager.GetCache(UserManageConsts.Third_Party_Ticket_Cache).Set(id, ComponentVerifyTicket, TimeSpan.FromMinutes(30));
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
    }
}
