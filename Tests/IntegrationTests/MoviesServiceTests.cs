using ApiApplication.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Tests.IntegrationTests;
using Xunit;

namespace Tests;

public class MoviesServiceTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _testFixture;
    public MoviesServiceTests(TestFixture fixture, TestFixture testFixture)
    {
        _testFixture = testFixture;
    }
    
    [Fact]
    public async Task GetAllMoviesTest()
    {
        await Task.Delay(10000);

        var movieService = _testFixture.ServiceProvider.GetRequiredService<IMoviesService>();
        
        var movies = await movieService.GetAllMoviesAsync();

        movies.Should().NotBeNull();
    }
}