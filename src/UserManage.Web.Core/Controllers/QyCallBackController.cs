using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tencent;
using UserManage.QyCallBack.DomainService;

namespace UserManage.Controllers
{
    [Route("api/[controller]/[action]")]
    public class QyCallBackController : UserManageControllerBase
    {
        private readonly IQyMsg _qyMsg;

        public QyCallBackController(IQyMsg qyMsg)
        {
            this._qyMsg = qyMsg;
        }

        [HttpGet("{tid}")]
        public async Task Qymsg(int tid, string msg_signature, string timestamp, string nonce, string echostr)
        {
            string msg= _qyMsg.VerifyUrl(msg_signature, timestamp, nonce, echostr);
             var accept = Request.GetTypedHeaders().Accept;
            var data = Encoding.UTF8.GetBytes(msg);
            if (accept.Any(x => x.MediaType == "text/html"))
            {
                Response.ContentType = "text/html";
            }
            else
            {
                Response.ContentType = "text/plain";
            }
            await Response.Body.WriteAsync(data, 0, data.Length);
        }

        [HttpPost("{tid}")]
        public void Qymsg(int tid,string msg_signature, string timestamp, string nonce)
        {

         
            StreamReader reader = new StreamReader(Request.Body);
            string content = reader.ReadToEnd();
            _qyMsg.DecryptContent(tid,msg_signature, timestamp, nonce, content);
            //string content = root["Content"].InnerText;
        }
    }
}
