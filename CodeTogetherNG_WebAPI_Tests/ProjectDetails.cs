using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTogetherNG_WebAPI_Tests
{
    class ProjectDetails
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public List<string> Member { get; set; }
        public string CreationDate { get; set; }
        public bool NewMembers { get; set; }
        public List<string> Technologies { get; set; }
        public string State { get; set; }
    }
}
