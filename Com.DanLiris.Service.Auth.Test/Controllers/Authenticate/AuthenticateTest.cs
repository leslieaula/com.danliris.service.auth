using Com.DanLiris.Service.Auth.Lib.Authentication;
using IdentityServer4.Test;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.DanLiris.Service.Auth.Test.Controllers.Authenticate
{
    [Collection("TestServerFixture Collection")]
    public class AuthenticateTest
    {
        private const string AUTH_URI = "v1/authenticate";
        private const string ME_URI = "v1/me";
        private List<string> Keys = new List<string> { "Permission", "Profile", "Username" };

        private TestServerFixture TestFixture { get; set; }

        public AuthenticateTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        private HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        [Fact]
        public async Task Should_Success_Login()
        {
            TestUser testUser = Config.GetTestUsers().First();
            testUser.Password = "Test";

            var response = await Client.PostAsync(AUTH_URI, new StringContent(JsonConvert.SerializeObject(testUser).ToString(), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            var json = response.Content.ReadAsStringAsync().Result;
            Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());

            Assert.True(result.ContainsKey("apiVersion"));
            Assert.True(result.ContainsKey("message"));
            Assert.True(result.ContainsKey("data"));
            Assert.True(result["data"].GetType().Name.Equals("String"));
        }

        /*
        [Fact]
        public async Task Should_Success_Get_Profile_From_Token()
        {
            var response = await Client.GetAsync(ME_URI);
            response.EnsureSuccessStatusCode();

            var json = response.Content.ReadAsStringAsync().Result;
            Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());

            Assert.True(result.ContainsKey("apiVersion"));
            Assert.True(result.ContainsKey("message"));
            Assert.True(result.ContainsKey("data"));
            Assert.True(result["data"].GetType().Name.Equals("JObject"));

            JObject error = (JObject)result["error"];
            foreach (string arg in Keys)
            {
                var prop = error.Properties().FirstOrDefault(c => c.Name.Equals(arg, StringComparison.CurrentCultureIgnoreCase));
                Assert.NotNull(prop);
            }
        }
        */
    }
}
