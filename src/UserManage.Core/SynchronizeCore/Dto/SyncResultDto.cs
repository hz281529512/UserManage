using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.SynchronizeCore.Dto
{
    /// <summary>
    /// 同步结果Dto
    /// </summary>
    public class SyncResultDto
    {
        /// <summary>
        /// 新建数
        /// </summary>
        public int? CreateCount { get; set; }

        /// <summary>
        /// 匹配数
        /// </summary>
        public int? MatchCount { get; set; }

        /// <summary>
        /// 删除数
        /// </summary>
        public int? DeleteCount { get; set; }
    }
}
