
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Abp.AutoMapper;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Tencent;
using UserManage.AbpExternalCore;
using UserManage.Configuration;
using UserManage.QyCallBack.Model;
using UserManage.SynchronizeCore.DomainService;

namespace UserManage.QyCallBack.DomainService
{
    public class QyMsg : IQyMsg
    {

        private readonly IConfigurationRoot _appConfiguration;
        private readonly ISynchronizeManager _synchronizeManager;

        public QyMsg(IHostingEnvironment env, ISynchronizeManager synchronizeManager)
        {
            this._synchronizeManager = synchronizeManager;

            _appConfiguration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName, env.IsDevelopment());
        }


        public string VerifyUrl(string msg_signature, string timestamp, string nonce, string echostr)
        {
            string token = _appConfiguration["CallBack:Token"];
            string aeskey = _appConfiguration["CallBack:EncodingAESKey"];
            string corpid = _appConfiguration["CallBack:CorpID"];
            WXBizMsgCrypt wxcpt = new WXBizMsgCrypt(token, aeskey, corpid);
            int ret = 0;
            string sEchoStr = "";
            ret = wxcpt.VerifyURL(msg_signature, timestamp, nonce, echostr, ref sEchoStr);
            return sEchoStr;

        }

        public int DecryptContent(int tid,string msg_signature, string timestamp, string nonce, string content)
        {
            try
            {

              
                string token = _appConfiguration["CallBack:Token"];
                string aeskey = _appConfiguration["CallBack:EncodingAESKey"];
                string corpid = _appConfiguration["CallBack:CorpID"];
                WXBizMsgCrypt wxcpt = new WXBizMsgCrypt(token, aeskey, corpid);
                string sMsg = ""; // 解析之后的明文
                int ret = 0;
                ret = wxcpt.DecryptMsg(msg_signature, timestamp, nonce, content, ref sMsg);
                var data = Converter(tid, sMsg);
                return ret;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        private QyMsgBase Converter(int tid, string xml) {

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNode root = doc.FirstChild;
            string changeType = root["ChangeType"].InnerText;
            switch (changeType.Split("_")[1])
            {
                case "user":
                    var user=  Deserialize<QyUserBase>(xml);
                    var userDto = Mapper.Map<AbpQYCallbackUser>(user);
                    _synchronizeManager.MatchSingleUserWithoutTenant(userDto, tid);
                    return user;
                case "party":
                    var party = Deserialize<QyPartyBase>(xml);
                    var dept = Mapper.Map<AbpWeChatDepartment>(party);
                     _synchronizeManager.MatchSingleDepartmentWithoutTenant(dept,tid);
                    return party;
                case "tag":
                    var tag = Deserialize<QyTagBase>(xml);
                    return tag;
                default:
                    return null;
            }


        }


        private static T Deserialize<T>(string xml)
        {
           
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(typeof(T));
                    return (T)xmldes.Deserialize(sr);
                }
          
        }
    }
}
