using ApiApplication.Services.Movies;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Tests.IntegrationTests;
using Xunit;

namespace Tests;

public class MoviesServiceTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _testFixture;
    public MoviesServiceTests(TestFixture testFixture)
    {
        _testFixture = testFixture;
    }
    
    [Fact]
    public async Task GetAllMoviesTest()
    {
        await Task.Delay(10000);

        var movieService = _testFixture.ServiceProvider.GetRequiredService<IMoviesService>();
        
        var movies = await movieService.GetAllMoviesAsync(CancellationToken.None);

        movies.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetMovieByIdTest()
    {
        await Task.Delay(10000);

        var movieService = _testFixture.ServiceProvider.GetRequiredService<IMoviesService>();
        
        var movie = await movieService.GetMovieByIdAsync("tt0111161",CancellationToken.None);

        movie.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetMovieByIdNotFoundTest()
    {
        await Task.Delay(10000);

        var movieService = _testFixture.ServiceProvider.GetRequiredService<IMoviesService>();
        
        var movie = await movieService.GetMovieByIdAsync("notExistingMovieId", CancellationToken.None);

        movie.Should().BeNull();
    }
}