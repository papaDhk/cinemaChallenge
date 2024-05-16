using Microsoft.Extensions.Configuration;

namespace Tests.IntegrationTests;

using ApiApplication;

using Microsoft.Extensions.DependencyInjection;
using Ductus.FluentDocker.Model.Common;
using Ductus.FluentDocker.Model.Compose;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Impl;

public class TestFixture : DockerComposeTestBase
{
    public ServiceProvider ServiceProvider { get; set; }
    public TestFixture()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("IntegrationTests/appsettings.json");
        IConfiguration configuration = configurationBuilder.Build();
        
        var serviceCollection = new ServiceCollection();
        new Startup(configuration).ConfigureCustomsService(serviceCollection);
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    protected override ICompositeService Build()
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(),
            (TemplateString)"IntegrationTests/docker-compose.yaml");

        return new DockerComposeCompositeService(
            DockerHost,
            new DockerComposeConfig
            {
                ComposeFilePath = new List<string> { file },
                ForceRecreate = true,
                RemoveOrphans = true,
                StopOnDispose = true
            });
    }
}