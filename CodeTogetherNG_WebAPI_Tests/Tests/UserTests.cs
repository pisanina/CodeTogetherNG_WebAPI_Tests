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
    public class UserTests : TestSetup
    {
        [Test]
        public async Task Register()
        {
            var user = new UserDto()
            {
                Username = "WebAPITest@User.com",
                Password = "WebAPITestPassword1!"
            };

            var userJson = JsonConvert.SerializeObject(user);
            var response = await httpClient.PostAsync(Configuration.WebApiUrl+"User/Register",
                new StringContent(userJson, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.Created);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public async Task Login()
        {
            var response = await base.LoginAsync();

            if (response.IsSuccessStatusCode)
            {
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public async Task CanLoginAfterRegister()
        {
            var user = new UserDto()
            {
                Username = "WebAPITest@User.com",
                Password = "WebAPITestPassword1!"
            };

            var userJson = JsonConvert.SerializeObject(user);
            var registerResponse = await httpClient.PostAsync(Configuration.WebApiUrl+"User/Register",
                new StringContent(userJson, Encoding.UTF8, "application/json"));


            var loginResponse = await httpClient.PostAsync(Configuration.WebApiUrl+"User/Login",
                new StringContent(userJson, Encoding.UTF8, "application/json"));

            if (loginResponse.IsSuccessStatusCode)
            {
                Assert.True(loginResponse.StatusCode == System.Net.HttpStatusCode.OK);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public async Task AddUserItRole() 
        {
            await Login();
            
            var response=await httpClient.PostAsync(Configuration.WebApiUrl+"User/ITRole", 
                            new StringContent("3", Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.Created);
                var check = await httpClient.GetAsync(Configuration.WebApiUrl+"User/26AEDED9-3796-450B-B891-03272C849854");
                if (check.IsSuccessStatusCode)
                {
                    var r = await check.Content.ReadAsAsync<Profile>();
                    Assert.True(r.UserITRole.Count() == 2);
                }
            }
            else
            {
                Assert.True(false, "Non success Conection");
            }
        }

        [Test]
        public async Task DeleteUserItRole()
        {
            await Login();
            var roleId = 1;

            var response=await httpClient.DeleteAsync(Configuration.WebApiUrl+"User/ITRole/"+roleId );

            if (response.IsSuccessStatusCode)
            {
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);

                var check = await httpClient.GetAsync(Configuration.WebApiUrl+"User/26AEDED9-3796-450B-B891-03272C849854");
                if (check.IsSuccessStatusCode)
                {
                    var r = await check.Content.ReadAsAsync<Profile>();
                    Assert.True(r.UserITRole.Count() == 0);
                }
            }
            else
            {
                Assert.True(false, "Non success Conection");
            }
        }


        [Test]
        public async Task AddUserTech()
        {
            await Login();

            var tech= new UsersTechnology
            {
                TechnologyId = 3,
                TechLevel = 1
            };

            var json = JsonConvert.SerializeObject(tech);

            var response=await httpClient.PostAsync(Configuration.WebApiUrl+"User/Tech/", 
                                         new StringContent(json, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.Created);

                var check = await httpClient.GetAsync(Configuration.WebApiUrl+"User/26AEDED9-3796-450B-B891-03272C849854");
                if (check.IsSuccessStatusCode)
                {
                    var r = await check.Content.ReadAsAsync<Profile>();
                    Assert.True(r.UserSkills.Count() == 4);
                    Assert.True(r.UserSkills[3].TechLevel == 1);
                }
            }
            else
            {
                Assert.True(false, "Non success Conection");
            }
        }

        [Test]
        public async Task DeleteUserTech()
        {
            await Login();
            var techId = 1;

            var response=await httpClient.DeleteAsync(Configuration.WebApiUrl+"User/Tech/"+techId );

            if (response.IsSuccessStatusCode)
            {
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);

                var check = await httpClient.GetAsync(Configuration.WebApiUrl+"User/26AEDED9-3796-450B-B891-03272C849854");
                if (check.IsSuccessStatusCode)
                {
                    var r = await check.Content.ReadAsAsync<Profile>();
                    Assert.True(r.UserSkills.Count() == 2);
                }
            }
            else
            {
                Assert.True(false, "Non success Conection");
            }
        }

        [Test]
        public async Task CheckDataInProfil()
        {
            string userId= "26AEDED9-3796-450B-B891-03272C849854";

            var response = await httpClient.GetAsync(Configuration.WebApiUrl+"User/"+userId);
            if (response.IsSuccessStatusCode)
            {
                var r = await response.Content.ReadAsAsync<Profile>();

                Assert.True(r.UserSkills.Count() == 3);
                Assert.True(r.UserSkills[0].TechName == "Angular");
                Assert.True(r.UserSkills[0].TechLevel == 1);
                Assert.True(r.UserOwner.Count() == 5);
                Assert.True(r.UserOwner[0].Id == 1);
                Assert.True(r.UserOwner[0].Title == "FirstProject");
            }
            else
            {
                Assert.True(false, "Non success Conection");
            }
        }

        [Test]
        public async Task UsersList()
        {
            var response = await httpClient.GetAsync(Configuration.WebApiUrl+"User");
            if (response.IsSuccessStatusCode)
            {
                var r = await response.Content.ReadAsAsync<List<User>>();

                Assert.True(r.Count() == 3);

                Assert.True(r[0].Id == "26AEDED9-3796-450B-B891-03272C849854");
                Assert.True(r[0].UserName == "TestUser@a.com");
                Assert.True(r[0].Owner == 5);
                Assert.True(r[0].Member == 0);
                Assert.True(r[0].Beginner == 1);
                Assert.True(r[0].Advanced == 1);
                Assert.True(r[0].Expert == 1);
            }
            else
            {
                Assert.True(false, "Non success Conection");
            }
        }



        [Test]
        public async Task TestJWTTokenLogged()
        {
            await Login();

            var response = await httpClient.GetAsync(Configuration.WebApiUrl+"Projects/UserName");
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
          
            var r = await response.Content.ReadAsAsync<string>();
            Assert.True(r =="TestUser@a.com");
        }

        [Test]
        public async Task TestJWTTokenNotLogged()
        {
            var response = await httpClient.GetAsync(Configuration.WebApiUrl+"Projects/UserName");
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        


        [TestCase(true, 1)]
        [TestCase(false, 0)]
        public async Task HandleRequest(bool accept, int membersCount)
        {
            await Login();

            var checkMembers = await httpClient.GetAsync(Configuration.WebApiUrl+"Projects/3");
            var r = await checkMembers.Content.ReadAsAsync<ProjectDetails>();
            Assert.AreEqual(0, r.Member.Count());

            var user = new HandleRequestDto()
            {
                ProjectId = 3,
                UserId = "E4DC8E37-CA29-46E9-8B3E-6FDCB18545F6",
                Accept = accept
            };

            var requestJson = JsonConvert.SerializeObject(user);
            
            var handleRequest = await httpClient.PutAsync(Configuration.WebApiUrl+"Projects/Request",
                new StringContent(requestJson, Encoding.UTF8, "application/json"));

            Assert.True(handleRequest.StatusCode == System.Net.HttpStatusCode.OK);

           

            checkMembers = await httpClient.GetAsync(Configuration.WebApiUrl+"Projects/3");
            r = await checkMembers.Content.ReadAsAsync<ProjectDetails>();
            Assert.AreEqual(membersCount, r.Member.Count());
        }

        [Test]
        public async Task HandleRequestNotOwner()
        {
            await LoginAsync(TestUsers.Coder);

            var user = new HandleRequestDto()
            {
                ProjectId = 3,
                UserId = "E4DC8E37-CA29-46E9-8B3E-6FDCB18545F6",
                Accept = true
            };

            var requestJson = JsonConvert.SerializeObject(user);

            var handleRequest = await httpClient.PutAsync(Configuration.WebApiUrl+"Projects/Request",
                new StringContent(requestJson, Encoding.UTF8, "application/json"));

            Assert.True(handleRequest.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }
    }
}
