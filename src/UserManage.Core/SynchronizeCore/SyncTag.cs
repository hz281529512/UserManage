using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.SynchronizeCore
{
    public class SyncTag
    {

        public string ChangeType { get; set; }
        public int? TagId { get; set; }

        public string AddUserItems { get; set; }

        public string DelUserItems { get; set; }

        public string AddPartyItems { get; set; }

        public string DelPartyItems { get; set; }
    }
}
