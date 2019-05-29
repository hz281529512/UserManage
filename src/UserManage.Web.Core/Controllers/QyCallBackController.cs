using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tencent;
using UserManage.QyCallBack.DomainService;
using UserManage.ThirdPartyConfigCore.DomainService;

namespace UserManage.Controllers
{
    [Route("api/[controller]/[action]")]
    public class QyCallBackController : UserManageControllerBase
    {
        private readonly IQyMsg _qyMsg;

        private readonly IThirdPartyConfigManager _tpManager;

        private readonly ILogger _logger;

        public QyCallBackController(
            IQyMsg qyMsg,
            IThirdPartyConfigManager tpManager
        )
        {
            this._qyMsg = qyMsg;
            this._tpManager = tpManager;
            _logger = NullLogger.Instance;
        }

        [HttpGet("{tid}")]
        public async Task Qymsg(int tid, string msg_signature, string timestamp, string nonce, string echostr)
        {
            string msg = _qyMsg.VerifyUrl(msg_signature, timestamp, nonce, echostr);
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
        public void Qymsg(int tid, string msg_signature, string timestamp, string nonce)
        {


            StreamReader reader = new StreamReader(Request.Body);
            string content = reader.ReadToEnd();
            _qyMsg.DecryptContent(tid, msg_signature, timestamp, nonce, content);
            //string content = root["Content"].InnerText;
        }

        [HttpGet("{tpid}")]
        public async Task Tpmsg(string tpid, string msg_signature, string timestamp, string nonce, string echostr)
        {

            //string msg = _tpManager.VerifyUrl(tpid, msg_signature, timestamp, nonce, echostr);

            //return Content("success : " + msg);
            var data = Encoding.UTF8.GetBytes("success");
            await Response.Body.WriteAsync(data, 0, data.Length);
        }

        [HttpPost("{tpid}")]
        public ActionResult Tpmsg(string tpid, string msg_signature, string timestamp, string nonce){
            var result = "";
            using (var streamReader = new StreamReader(Request.Body))
            {
                string stringInput = streamReader.ReadToEnd();
                _logger.Info(stringInput);
                result = _tpManager.VerifySuiteTicket(tpid, msg_signature, timestamp, nonce, stringInput);
                return Content(result);
            }
        }
    }
}
