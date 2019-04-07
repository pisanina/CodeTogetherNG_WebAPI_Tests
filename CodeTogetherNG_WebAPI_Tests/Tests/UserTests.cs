using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CodeTogetherNG_WebAPI_Tests.DTOs;
using CodeTogetherNG_WebAPI_Tests.Plumbing;
using Newtonsoft.Json;
using NUnit.Framework;

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
            var response = await base.Login();

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
    }
}
