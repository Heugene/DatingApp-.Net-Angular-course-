using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class MemberParams : PagingParams
    {
        public string? Gender { get; set; }
        public string? CurrentMemberId { get; set; }
    }
}