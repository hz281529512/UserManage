using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.SynchronizeCore.Dtos
{
    public class ManyMatchResultDto
    {
        public Dictionary<string, MatchResultDto> results { get; set; }

        public ManyMatchResultDto()
        {
            results = new Dictionary<string, MatchResultDto>();
        }
    }
}
