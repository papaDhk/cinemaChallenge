using Ductus.FluentDocker.Model.Common;
using Ductus.FluentDocker.Model.Compose;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Impl;

namespace IntegrationTests;

public class DockerComposeTestBase : IDisposable
{
    private ICompositeService _compositeService;
    private IHostService? _dockerHost;

    public DockerComposeTestBase()
    {
        EnsureDockerHost();

        _compositeService = Build();
        try
        {
            _compositeService.Start();
        }
        catch(Exception e)
        {
            _compositeService.Dispose();
            throw;
        }
    }

    public void Dispose()
    {
        var compositeService = _compositeService;
        _compositeService = null!;
        try
        {
            compositeService?.Dispose();
        }
        catch
        {
            // ignored
        }
    }

    public ICompositeService Build()
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(),
            (TemplateString)"IntegrationTests/docker-compose.yaml");

        return new DockerComposeCompositeService(
            _dockerHost,
            new DockerComposeConfig
            {
                ComposeFilePath = new List<string> { file },
                ForceRecreate = true,
                RemoveOrphans = true,
                StopOnDispose = true
            });
    }

    private void EnsureDockerHost()
    {
        if (_dockerHost?.State == ServiceRunningState.Running) return;

        var hosts = new Hosts().Discover();
        _dockerHost = hosts.FirstOrDefault(x => x.IsNative) ?? hosts.FirstOrDefault(x => x.Name == "default");

        if (null != _dockerHost)
        {
            if (_dockerHost.State != ServiceRunningState.Running) _dockerHost.Start();

            return;
        }

        if (hosts.Count > 0) _dockerHost = hosts.First();

        if (null != _dockerHost) return;

        EnsureDockerHost();
    }
}