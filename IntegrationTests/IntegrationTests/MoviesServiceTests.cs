using ApiApplication.Services;
using ApiApplication.Services.Movies;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTests;

public class MoviesServiceTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _testFixture;
    public MoviesServiceTests(TestFixture testFixture)
    {
        _testFixture = testFixture;
    }
    
    [Fact]
    public async Task GetAllMoviesAsync_ShouldReturnMovies()
    {
        await Task.Delay(10000);

        var movieService = _testFixture.ServiceProvider.GetRequiredService<IMoviesService>();
        
        var movies = await movieService.GetAllMoviesAsync(CancellationToken.None);

        movies.Should().NotBeNull();
        movies.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task GetMovieByIdAsync_ShouldReturnMovie_WhenItExistInExternalMovieService()
    {
        await Task.Delay(10000);

        var movieService = _testFixture.ServiceProvider.GetRequiredService<IMoviesService>();

        var imdbId = "tt0111161";
        var movie = await movieService.GetMovieByIdAsync(imdbId,CancellationToken.None);

        movie.Should().NotBeNull();
        movie.ImdbId.Should().Be(imdbId);
        movie.Id.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public async Task GetMovieByIdAsync_ShouldThrowNotFoundException_WhenTheMovieDoesNotExistInExternalService()
    {
        await Task.Delay(10000);

        var movieService = _testFixture.ServiceProvider.GetRequiredService<IMoviesService>();
        
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await movieService.GetMovieByIdAsync("notExistingMovieId", CancellationToken.None));

    }
}