using Com.DanLiris.Service.Auth.Lib.Authentication;
using Com.DanLiris.Service.Auth.Lib.ViewModels;
using IdentityServer4.Test;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.DanLiris.Service.Auth.Test.Controllers.Account
{
    [Collection("TestFixture Collection")]
    public class AccountBasicTest
    {
        private const string URI = "v1/accounts";

        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public AccountBasicTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        public AccountViewModel GenerateNewData()
        {
            string guid = Guid.NewGuid().ToString();

            return new AccountViewModel()
            {
                username = guid,
                password = "Password",
                isLocked = false,
                profile = new AccountProfileViewModel { firstname = "FirstName", lastname = "LastName", gender = "Male" },
                roles = new List<RoleViewModel>()
            };
        }
        
        public async Task Get()
        {
            var response = await this.Client.GetAsync(URI);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        public async Task GetById()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        public async Task Post()
        {
            TestUser testUser = Config.GetUsers().First();

            var test = await Client.PostAsync("/v1/authenticate", new StringContent(JsonConvert.SerializeObject(testUser).ToString(), Encoding.UTF8, "application/json"));

            AccountViewModel accountVM = GenerateNewData();
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(accountVM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }    
    }
}
