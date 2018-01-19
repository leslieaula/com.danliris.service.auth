using Com.DanLiris.Service.Auth.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using Xunit;

namespace Com.DanLiris.Service.Auth.Test
{
    public class TestServerFixture : IDisposable
    {
        private readonly TestServer _server;

        public HttpClient Client { get; }

        public TestServerFixture()
        {
            string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(projectPath)
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new WebHostBuilder()
                .UseUrls("http://127.0.0.1:5000")
                .UseConfiguration(configuration)
                .UseStartup<Startup>();
            
            _server = new TestServer(builder);
            _server.CreateWebSocketClient();
            Client = _server.CreateClient();
            Client.BaseAddress = new Uri("http://127.0.0.1:5000");
        }

        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }
    }

    [CollectionDefinition("TestFixture Collection")]
    public class TestFixtureCollection : ICollectionFixture<TestServerFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
