﻿using System.Collections.Generic;

namespace CodeTogetherNG_WebAPI_Tests
{
    internal class Profile
    {
        public List<Skill> UserSkills;
        public List<UsersProjects> UserOwner;
        public List<UsersProjects> UserMember;
    }
}