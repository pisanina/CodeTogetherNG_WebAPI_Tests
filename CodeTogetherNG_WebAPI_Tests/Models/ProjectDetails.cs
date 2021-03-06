﻿using CodeTogetherNG_WebAPI_Tests.DTOs;
using System.Collections.Generic;

namespace CodeTogetherNG_WebAPI_Tests.Models
{
    internal class ProjectDetails
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public MemberDto Owner { get; set; }
        public List<MemberDto> Member { get; set; }
        public string CreationDate { get; set; }
        public bool NewMembers { get; set; }
        public List<int> Technologies { get; set; }
        public int State { get; set; }
    }
}