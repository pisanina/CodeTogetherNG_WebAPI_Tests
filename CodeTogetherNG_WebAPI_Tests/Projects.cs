﻿using System.Collections.Generic;

namespace CodeTogetherNG_WebAPI_Tests
{
    internal class Projects
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool? NewMembers { get; set; }
        public List<string> Technologies { get; set; }
    }
}