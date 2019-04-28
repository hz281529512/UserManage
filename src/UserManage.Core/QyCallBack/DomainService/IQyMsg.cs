using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Services;

namespace UserManage.QyCallBack.DomainService
{
    public interface IQyMsg:IDomainService
    {
        /// <summary>
        /// 验证url
        /// </summary>
        /// <param name="msg_signature">企业微信加密签名</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="nonce">随机数</param>
        /// <param name="echostr">加密的字符串</param>
        /// <returns></returns>
        string VerifyUrl(string msg_signature, string timestamp, string nonce, string echostr);
        /// <summary>
        /// 接受内容解密
        /// </summary>
        /// <param name="msg_signature">企业微信加密签名</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="nonce">随机数</param>
        /// <param name="content">加密内容</param>
        /// <returns></returns>
        int DecryptContent(string msg_signature, string timestamp, string nonce, string content);
    }
}
