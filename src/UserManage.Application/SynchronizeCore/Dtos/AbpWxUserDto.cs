using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.SynchronizeCore.Dtos
{
    public class AbpWxUserDto
    {
        public long? AbpUserId { get; set; }

        public long? AbpRelationId { get; set; }

        public string AbpUserName { get; set; }

        public string wx_userid { get; set; }
        public string wx_name { get; set; }

        public string wx_mobile { get; set; }

        public string wx_gender { get; set; }

        public string wx_email { get; set; }

        public string wx_avatar { get; set; }

        public string wx_alias { get; set; }

        public string wx_position { get; set; }

        public string wx_qr_code { get; set; }
    }
}
