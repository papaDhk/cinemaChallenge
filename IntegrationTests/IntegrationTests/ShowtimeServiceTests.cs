using ApiApplication.Services.Showtimes;
using ApiApplication.Services.Showtimes.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTests;

public class ShowtimeServiceTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _testFixture;
    public ShowtimeServiceTests(TestFixture testFixture)
    {
        _testFixture = testFixture;
    }
    
    [Fact]
    public async Task CreateShowTimeTest()
    {
        await Task.Delay(10000);

        var showtimesService = _testFixture.ServiceProvider.GetRequiredService<IShowtimesService>();

        var movieImDbId = "tt0111161";
        var showtime = await showtimesService.CreateShowtime(new ShowTimeCreationParameters
        {
            AuditoriumId = 1,
            MovieImDbId = movieImDbId,
            SessionDate = DateTime.UtcNow.AddDays(1),
        }, CancellationToken.None);

        showtime.Should().NotBeNull();

        var existingShowtime = await showtimesService.GetShowtimeByIdAsync(showtime.Id);
        
        existingShowtime.Should().NotBeNull();
        existingShowtime.Id.Should().Be(showtime.Id);
        showtime.Movie.ImdbId.Should().Be(movieImDbId);

        var allShowTimes = await showtimesService.GetAllAsync();
        allShowTimes.Should().NotBeEmpty();

    }
}