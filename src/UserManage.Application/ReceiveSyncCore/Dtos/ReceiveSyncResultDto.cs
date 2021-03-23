using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.ReceiveSyncCore.Dtos
{
    public class ReceiveSyncResultDto
    {
        public int Code { get; set; }

        public string Msg { get; set; }

        public ReceiveSyncResultDto(int _code , string _msg)
        {
            this.Code = _code;
            this.Msg = _msg;
        }
    }
}
