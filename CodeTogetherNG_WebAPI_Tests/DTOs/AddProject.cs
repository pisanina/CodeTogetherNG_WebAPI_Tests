using System.Collections.Generic;

namespace CodeTogetherNG_WebAPI_Tests.DTOs
{
    internal class AddProject
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool? NewMembers { get; set; }
        public List<int> Technologies { get; set; }
    }
}