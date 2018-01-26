using Com.DanLiris.Service.Auth.Lib.Authentication;
using Com.DanLiris.Service.Auth.Test.DataUtils;
using Com.DanLiris.Service.Auth.WebApi;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Xunit;

namespace Com.DanLiris.Service.Auth.Test
{
    public class TestServerFixture : IDisposable
    {
        private readonly TestServer _server;
        public HttpClient Client { get; }
        public IServiceProvider Service { get; }

        public TestServerFixture()
        {
            string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(projectPath)
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new WebHostBuilder()
                .UseConfiguration(configuration)
                .ConfigureServices(services =>
                {
                    services
                        .AddTransient<AccountDataUtil>()
                        .AddTransient<AccountProfileDataUtil>()
                        .AddTransient<PermissionDataUtil>()
                        .AddTransient<RoleDataUtil>();
                })
                .UseStartup<Startup>();

            string authority = configuration["Authority"];
            string clientId = configuration["ClientId"];
            string secret = configuration["Secret"];

            _server = new TestServer(builder);
            Client = _server.CreateClient();
            Service = _server.Host.Services;

            /* Get Token */
            TestUser testUser = Config.GetTestUsers().First();
            var response = Client.PostAsync("v1/authenticate", new StringContent(JsonConvert.SerializeObject(testUser).ToString(), Encoding.UTF8, "application/json")).Result;
            response.EnsureSuccessStatusCode();

            var data = response.Content.ReadAsStringAsync();
            Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(data.Result.ToString());
            var token = result["data"].ToString();

            Client.SetBearerToken(token);
        }

        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }
    }

    [CollectionDefinition("TestServerFixture Collection")]
    public class TestServerFixtureCollection : ICollectionFixture<TestServerFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
