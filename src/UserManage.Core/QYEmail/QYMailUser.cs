using System;
using System.Collections.Generic;
using System.Text;
using UserManage.AbpExternalCore.Model;

namespace UserManage.QYEmail
{


    public class QYMailCheckUser : AbpWechatResult
    {
        public List<QYMailCheckUserDtl> list { get; set; }
    }
    public class QYMailCheckUserDtl
    {
        public string user { get; set; }

        public int type { get; set; }
    }

    internal class QYMailUser : AbpWechatResult
    {
        /// <summary>
        /// 成员列表
        /// </summary>
        public List<QYMailUserDtl> userlist { get; set; }
    }

    internal class QYMailUserDtl
    {
        /// <summary>
        /// 成员UserI[Email]
        /// </summary>
        public string userid { get; set; }

        /// <summary>
        /// 成员名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 成员所属部门
        /// </summary>
        public List<int> department { get; set; }
    }
}
