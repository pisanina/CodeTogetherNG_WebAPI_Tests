using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CodeTogetherNG_WebAPI_Tests.DTOs;
using CodeTogetherNG_WebAPI_Tests.Models;
using CodeTogetherNG_WebAPI_Tests.Plumbing;

namespace CodeTogetherNG_WebAPI_Tests.Tests
{
    public class ProjectTests : TestSetup
    {
        [Test]
        public async Task Search()
        {
            var response = await httpClient.GetAsync(Configuration.WebApiUrl+"Projects/");
            if (response.IsSuccessStatusCode)
            {
                var r = await response.Content.ReadAsAsync<IEnumerable<Projects>>();

                Assert.True(r.Count() == 6);
                // Assert.True(r.Select(p => p.Title.Contains("FirstProject"));
            }
            else
            {
                Assert.True(false, "Non success Conection");
            }
        }

        [TestCase("Funny", 1)]
        [TestCase("FUNNY", 1)]
        [TestCase("pamięć", 1)]
        [TestCase("ęć", 1)]
        [TestCase("", 6)]
        [TestCase("Project that not exist", 0)]
        [TestCase("Our first project will be SUPRISE Hello World", 1)]
        [TestCase("',''); <script>alert('BUM!');</script>'", 0)]
        public async Task SearchByTitleOrDescription(string toSearch, int resultsCount)
        {
            var response = await httpClient.GetAsync(Configuration.WebApiUrl+"Projects/?toSearch="+toSearch);
            if (response.IsSuccessStatusCode)
            {
                var r = await response.Content.ReadAsAsync<IEnumerable<Projects>>();

                Assert.True(r.Count() == resultsCount);
                if (resultsCount != 0)
                    Assert.True(r.Any(p => p.Title.Contains(toSearch, StringComparison.InvariantCultureIgnoreCase))
                                || r.Any(p => p.Description.Contains(toSearch, StringComparison.InvariantCultureIgnoreCase)));
            }
            else
            {
                Assert.True(false, "Non success Conection");
            }
        }

        [TestCase("E4DC8E37-CA29-46E9-8B3E-6FDCB18545F6", 0, 1, 2)]
        [TestCase("E5EB52F2-5B58-48D3-BCCD-47A3E9E27175", 0, 0, 0)]
        [TestCase("26AEDED9-3796-450B-B891-03272C849854", 3, 5, 0)]
        public async Task ShowUserProfile(string userId, int skillsCount, int ownerCount, int memberCount)
        {
            var response = await httpClient.GetAsync(Configuration.WebApiUrl+"User/"+userId);
            if (response.IsSuccessStatusCode)
            {
                var r = await response.Content.ReadAsAsync<Profile>();

                Assert.True(r.UserSkills.Count() == skillsCount);
                Assert.True(r.UserOwner.Count() == ownerCount);
                Assert.True(r.UserMember.Count() == memberCount);
            }
            else
            {
                Assert.True(false, "Non success Conection");
            }
        }


        [Test]
        public async Task TechnologyList()
        {
            var response = await httpClient.GetAsync(Configuration.WebApiUrl+"TechList");
            if (response.IsSuccessStatusCode)
            {
                var r = await response.Content.ReadAsAsync<List<Technology>>();

                Assert.True(r.Count() == 18);
                Assert.True(r[0].Id == 1);
                Assert.True(r[0].techName == "Angular");
                Assert.True(r[17].Id == 18);
                Assert.True(r[17].techName == "Visual Basic");
            }
            else
            {
                Assert.True(false, "Non success Conection");
            }
        }

        [Test]
        public async Task ProjectDetails()
        {
            var response = await httpClient.GetAsync(Configuration.WebApiUrl+"Projects/Details?id=5");
            if (response.IsSuccessStatusCode)
            {
                var r = await response.Content.ReadAsAsync<ProjectDetails>();

                Assert.True(r.Title == "Test for adding Project with four Tech");
                Assert.True(r.Description == "Test for adding Project with four Technologies");
                Assert.True(r.Owner == "TestUser@a.com");
                Assert.True(r.Member[0] == "newcoder@a.com");
                Assert.True(r.NewMembers == false);
                Assert.True(r.CreationDate == "24/02/2019");
                Assert.True(r.Technologies[0] == "Assembly");
                Assert.True(r.Technologies[1] == "C++");
                Assert.True(r.Technologies[2] == "Java");
                Assert.True(r.Technologies[3] == "JavaScript");
                Assert.True(r.State == " Preparing");
            }
            else
            {
                Assert.True(false, "Non success Conection");
            }
        }

        

        [Test]
        public async Task AddProject()
        {
            await Login();

            var project= new AddProject
            {
                Title= "NewProject from API",
                Description = "Description for WebAPI",
                NewMembers= false,
                Technologies = new List<int>{1,7}
            };

            var json = JsonConvert.SerializeObject(project);
            var response=await httpClient.PostAsync(Configuration.WebApiUrl+"Projects",
                new StringContent(json, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.Created);
            }
            else
            {
                Assert.True(false, "Non success Conection");
            }
        }

        [Test]
        public async Task DeleteProject()
        {
            await Login();
            var projectId = 6;

            var response=await httpClient.DeleteAsync(Configuration.WebApiUrl+"Projects/Delete/"+projectId );

            if (response.IsSuccessStatusCode)
            {
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
            }
            else
            {
                Assert.True(false, "Non success Conection");
            }
        }
    }
}