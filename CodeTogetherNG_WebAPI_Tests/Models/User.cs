﻿using System.Collections.Generic;

namespace CodeTogetherNG_WebAPI_Tests.Models
{
    internal class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int Owner { get; set; }
        public int Member { get; set; }
        public int Beginner { get; set; }
        public int Advanced { get; set; }
        public int Expert { get; set; }
        public int ITRole { get; set; }
    }
}