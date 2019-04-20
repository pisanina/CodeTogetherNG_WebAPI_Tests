using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTogetherNG_WebAPI_Tests.Models
{
    class Request
    {
        public string MemberId { get; set; }
        public string MemberName { get; set; }
        public string Message { get; set; }
        public bool? Accepted { get; set; }
    }
}
