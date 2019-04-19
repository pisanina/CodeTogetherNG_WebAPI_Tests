using CodeTogetherNG_WebAPI_Tests.DTOs;
using CodeTogetherNG_WebAPI_Tests.Models;
using CodeTogetherNG_WebAPI_Tests.Plumbing;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CodeTogetherNG_WebAPI_Tests.Tests
{
    public class ProjectTests : TestSetup
    {
        [Test]
        public async Task GetAllProjects()
        {
            var response = await httpClient.GetAsync(Configuration.WebApiUrl+"Projects/");
            if (response.IsSuccessStatusCode)
            {
                var r = await response.Content.ReadAsAsync<List<Projects>>();

                Assert.True(r.Count() == 6);
                Assert.True(r[0].Title == "FirstProject");
                Assert.True(r[0].Description == "Our first project will be SUPRISE Hello World");
                Assert.True(r[0].NewMembers == false);
                Assert.True(r[0].Technologies.Count() == 0);

                Assert.True(r[5].Title == "Project with Two Tech");
                Assert.True(r[5].Description == "Project with Two Tech (Test)");
                Assert.True(r[5].NewMembers == false);
                Assert.True(r[5].Technologies.Count() == 2);
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
        [TestCase("<script>alert('BUM!');</script>", 1)]
        [TestCase("',''); CREATE LOGIN Admin WITH PASSWORD = 'ABCD'--", 1)]
        [TestCase("<h1>html injection</h1>", 1)]
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

        [Test]
        public async Task SearchByProjectState()
        {
            int state = 2;
            var response = await httpClient.GetAsync(Configuration.WebApiUrl+"Projects/?projectState="+state);
            Assert.True(response.IsSuccessStatusCode);

            var r = await response.Content.ReadAsAsync<List<Projects>>();

            Assert.True(r.Count() == 1);
            Assert.True(r[0].State == "In Progress");
        }

        [Test]
        public async Task SearchByNewMembers()
        {
            bool newMembers = true;
            var response = await httpClient.GetAsync(Configuration.WebApiUrl+"Projects/?newMembers="+newMembers);

            Assert.True(response.IsSuccessStatusCode);

            var r = await response.Content.ReadAsAsync<List<Projects>>();

            Assert.True(r.Count() == 1);
            Assert.True(r[0].NewMembers == true);
        }


        [Test]
        public async Task SearchByTechnology()
        {
            List<int> techs = new List<int>{4,7};
           
            var response = await httpClient.GetAsync(Configuration.WebApiUrl+"Projects/?techList="+techs[0]+"&techList="+techs[1]);

            Assert.True(response.IsSuccessStatusCode);

            var r = await response.Content.ReadAsAsync<List<Projects>>();

            Assert.True(r.Count() == 1);
            Assert.True(r[0].Technologies.Count() == 4);
        }


        [TestCase("E4DC8E37-CA29-46E9-8B3E-6FDCB18545F6", 0, 1, 2)]
        [TestCase("E5EB52F2-5B58-48D3-BCCD-47A3E9E27175", 0, 0, 0)]
        [TestCase("26AEDED9-3796-450B-B891-03272C849854", 3, 5, 0)]
        public async Task ShowUsers(string userId, int skillsCount, int ownerCount, int memberCount)
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
            var response = await httpClient.GetAsync(Configuration.WebApiUrl+"Projects/Details?id=5");//1. zrobmy to bardziej rest: Projects/5
            if (response.IsSuccessStatusCode)
            {
                var r = await response.Content.ReadAsAsync<ProjectDetails>();

                Assert.True(r.Title == "Test for adding Project with four Tech");
                Assert.True(r.Description == "Test for adding Project with four Technologies <h1>html injection</h1>");
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

                var projectsList = await httpClient.GetAsync(Configuration.WebApiUrl+"Projects/");
                var r = await projectsList.Content.ReadAsAsync<IEnumerable<Projects>>();

                Assert.True(r.Count() == 7);
            }
            else
            {
                Assert.Fail("Non success Conection");
            }
        }

        [Test]
        public async Task AddProjectNotLogged()
        {

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
                Assert.Fail("Should have no rights!");
            }
            else
            {
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
            }
        }

        [Test]
        public async Task ChangeProject() 
        {
            await Login();
            var projectId = 5;

            var project= new ChangeProject
            {
                ProjectId = projectId,
                Title= "Project changed by webAPI",
                Description = "Description for Project changed by webAPI",
                NewMembers= false,
                State = 2,
                Technologies = new List<int>{1}
            };

            var json = JsonConvert.SerializeObject(project);
            var response=await httpClient.PutAsync(Configuration.WebApiUrl+"Projects/Change/",
                new StringContent(json, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);

                var check = await httpClient.GetAsync(Configuration.WebApiUrl+"Projects/Details?id="+projectId);
                if (check.IsSuccessStatusCode)
                {
                    var r = await check.Content.ReadAsAsync<ProjectDetails>();
                    Assert.True(r.Title == project.Title);
                    Assert.True(r.Description == project.Description);
                    Assert.True(r.Technologies.Count() == 1);
                }
            }
            else
            {
                Assert.Fail("Non success Conection");
            }
        }

        [Test]
        public async Task ChangeProjectNotOwner()
        {
            await Login(TestUsers.Coder);
            var projectId = 5;

            var project= new ChangeProject
            {
                ProjectId = projectId,
                Title= "Project changed by webAPI",
                Description = "Description for Project changed by webAPI",
                NewMembers= false,
                State = 2,
                Technologies = new List<int>{1}
            };

            var json = JsonConvert.SerializeObject(project);
            var response=await httpClient.PutAsync(Configuration.WebApiUrl+"Projects/Change/",
                new StringContent(json, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("Should have no rights!");
            }
            else
            {
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
            }
        }


        [Test]
        public async Task ChangeProjectNotLogged()
        {
            var projectId = 5;

            var project= new ChangeProject
            {
                ProjectId = projectId,
                Title= "Project changed by webAPI",
                Description = "Description for Project changed by webAPI",
                NewMembers= false,
                State = 2,
                Technologies = new List<int>{1}
            };

            var json = JsonConvert.SerializeObject(project);
            var response=await httpClient.PutAsync(Configuration.WebApiUrl+"Projects/Change/",
                new StringContent(json, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("Should have no rights!");
            }
            else
            {
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
            }
        }

        [Test]
        public async Task DeleteProject()
        {
            await Login();
            var projectId = 5;

            var response=await httpClient.DeleteAsync(Configuration.WebApiUrl+"Projects/Delete/"+projectId );

            if (response.IsSuccessStatusCode)
            {
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);

                var projectsList = await httpClient.GetAsync(Configuration.WebApiUrl+"Projects/");
                var r = await projectsList.Content.ReadAsAsync<IEnumerable<Projects>>();

                Assert.True(r.Count() == 5);
            }
            else
            {
                Assert.Fail();
            }
        }


        [Test]
        public async Task DeleteProjectNotOwner()
        {
            await Login(TestUsers.Coder);
            var projectId = 5;

            var response=await httpClient.DeleteAsync(Configuration.WebApiUrl+"Projects/Delete/"+projectId );

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("Should have no rights!");
            }
            else
            {
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
            }
        }

        [Test]
        public async Task DeleteProjectNotLogged()
        {
            var projectId = 5;

            var response=await httpClient.DeleteAsync(Configuration.WebApiUrl+"Projects/Delete/"+projectId );

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("Should have no rights!");
            }
            else
            {
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
            }
        }
    }
}