using ApiApplication;
using IntegrationTests;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.Endpoints;

public class EndpointTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private DockerComposeTestBase _dockerCompose;
    public Task InitializeAsync()
    {
        _dockerCompose = new DockerComposeTestBase();
        _dockerCompose.Build();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _dockerCompose.Dispose();
        return Task.CompletedTask;
    }
}