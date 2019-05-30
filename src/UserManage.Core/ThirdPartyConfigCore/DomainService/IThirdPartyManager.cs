

using System;
using System.Threading.Tasks;
using Abp;
using Abp.Domain.Services;
using UserManage.ThirdPartyConfigCore;


namespace UserManage.ThirdPartyConfigCore.DomainService
{
    public interface IThirdPartyManager : IDomainService
    {

        /// <summary>
        /// 获取第三方配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ThirdPartyConfig GetConfig(string id);



        /// <summary>
        /// 验证url
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <param name="msg_signature">企业微信加密签名</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="nonce">随机数</param>
        /// <param name="echostr">加密的字符串</param>
        /// <returns></returns>
        string VerifyUrl(string id, string msg_signature, string timestamp, string nonce, string echostr);


        /// <summary>
        /// 解析Ticket
        /// </summary>
        /// <param name="id"></param>
        /// <param name="signature"></param>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        /// <param name="stringInput"></param>
        /// <returns></returns>
        string VerifySuiteTicket(string id,string signature, string timestamp, string nonce, string stringInput);

    }
}
